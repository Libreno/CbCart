using CbCart.Repository;
using CbCart.Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CbCart.Daemon
{
	public class CbDaemon
	{
		private readonly string reportsFolder;
		private readonly int daysCount;
		private readonly string redisUrl;

		public CbDaemon(int daysCount, string reportsFolder, string redisUrl)
		{
			this.daysCount = daysCount;
			this.reportsFolder = reportsFolder;
			this.redisUrl = redisUrl;
		}

		public void Run()
		{
			using var repo = new CbCartRepository(redisUrl);
			var allCarts = repo.GetAllCarts();
			var cartsAlive = DeleteOldCarts(repo, allCarts, out var webHookTasks);
			var report = BuildReport(repo, cartsAlive);
			var path = $"{reportsFolder}carts_report_{DateTime.Now:yyyyMMdd_Hmmss}.txt";
			File.WriteAllText(path, report);
			Console.WriteLine(report);
			try
			{
				Task.WaitAll(webHookTasks.ToArray());
			}
			catch (Exception e) {
				Console.WriteLine($"Exception {e.Message}");
			}
			Console.WriteLine($"Report saved to '{path}'.");
		}

		private IEnumerable<string> DeleteOldCarts(CbCartRepository repo, IEnumerable<string> allCarts, out List<Task> webHookTasks)
		{
			var res = new List<string>();
			webHookTasks = new List<Task>();
			foreach (var cartId in allCarts)
			{
				var cart = repo.GetCart(cartId);
				Console.WriteLine($"Cart Id: {cartId}");
				if ((DateTime.UtcNow - cart.Created).TotalDays >= daysCount)
				{
					if (!string.IsNullOrWhiteSpace(cart.WebHook))
					{
						webHookTasks.Add(Task.Run(() => { 
							RunWebHook(cart); 
						}));
					}
					repo.DeleteCart(cartId);
					Console.WriteLine("deleted");
				}
				else
				{
					res.Add(cartId);
				}
			}
			return res;
		}

		private static void RunWebHook(Cart cart)
		{
			try
			{
				var req = WebRequest.Create(cart.WebHook);
				req.Method = "POST";
				var postData = JsonSerializer.Serialize(cart);
				Console.WriteLine(postData);
				var byteArr = Encoding.UTF8.GetBytes(postData);
				req.ContentType = "application/x-www-form-urlencoded";
				req.ContentLength = byteArr.Length;
				var dataStream = req.GetRequestStream();
				dataStream.Write(byteArr, 0, byteArr.Length);
				dataStream.Close();
				var resp = req.GetResponse();
				Console.WriteLine(((HttpWebResponse)resp).StatusDescription);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception {e.Message}");
			}
		}

		private string BuildReport(CbCartRepository repo, IEnumerable<string> carts)
		{
			GetReportData(repo, carts, 
				out int cartsWithBonusProducts, 
				out int expire10, 
				out int expire20, 
				out int expire30, 
				out decimal avgCheck);
			var reportBuilder = new StringBuilder();
			reportBuilder.AppendLine($"Total number of carts: {carts.Count()}");
			reportBuilder.AppendLine($"Number of carts with bonus products: {cartsWithBonusProducts}");
			reportBuilder.AppendLine($"Number of carts expiring within 10 days: {expire10}");
			reportBuilder.AppendLine($"Number of carts expiring within 20 days: {expire20}");
			reportBuilder.AppendLine($"Number of carts expiring within 30 days: {expire30}");
			reportBuilder.AppendLine($"Average check: {avgCheck}");
			return reportBuilder.ToString();
		}

		private void GetReportData(CbCartRepository repo, IEnumerable<string> carts, out int cartsWithBonusProducts, out int expire10, out int expire20, out int expire30, out decimal avgCheck)
		{
			cartsWithBonusProducts = 0;
			expire10 = 0;
			expire20 = 0;
			expire30 = 0;
			avgCheck = 0;
			decimal totalCost = 0;
			foreach (var cartId in carts)
			{
				var cart = repo.GetCart(cartId);
				var withBonusProd = false;
				foreach (var position in cart.Positions)
				{
					var product = repo.GetProduct(position.Item1);
					if (product.ForBonusPoints && !withBonusProd)
					{
						withBonusProd = true;
						cartsWithBonusProducts++;
					}
					totalCost += position.Item2 * product.Cost;
					//Console.WriteLine("product.Cost " + product.Cost);
					//Console.WriteLine("totalCost " + totalCost);
				}
				var daysToExpire = daysCount - (DateTime.UtcNow - cart.Created).TotalDays;
				if (daysToExpire <= 10)
				{
					expire10++;
				}
				else if (daysToExpire <= 20)
				{
					expire20++;
				}
				else if (daysToExpire <= 30)
				{
					expire30++;
				}
			}
			avgCheck = carts.Count() == 0 ? 0 : Math.Round(totalCost / carts.Count(), 2);
		}
	}
}
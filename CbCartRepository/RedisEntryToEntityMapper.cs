using StackExchange.Redis;
using CbCart.Repository.Entities;
using System;
using System.Linq;

namespace CbCart.Repository
{
	public static class RedisEntryToEntityMapper
	{
		public static Cart CreateCart(string cartId, HashEntry[] entries) {
			var dateCreated = DateTime.Parse(entries.Single(e => e.Name == CbCartRepository.CREATE_DATE).Value);
			var positions = entries.Where(e => e.Name.StartsWith(CbCartRepository.PRODUCT)).Select(e => {
				var name = e.Name.ToString();
				var productId = name.Substring(name.IndexOf(":") + 1);
				e.Value.TryParse(out long count);
				return new Tuple<string, int>(productId, (int) count);
			});
			var url = entries.FirstOrDefault(e => e.Name == CbCartRepository.WEBHOOK);
			return new Cart() { 
				Id = cartId,
				Created = dateCreated,
				Positions = positions,
				WebHook = url == null? string.Empty : url.Value.ToString()
			};
		}

		public static Product CreateProduct(HashEntry[] entries)
		{
			var isForBonus = entries.Single(he => he.Name == CbCartRepository.F_FOR_BONUS_POINTS).Value.ToString().ToUpperInvariant() == "TRUE";
			double.TryParse(entries.Single(he => he.Name == CbCartRepository.F_COST).Value.ToString(), out double cost);
			Console.WriteLine(cost);
			return new Product()
			{
				Cost = (decimal)cost,
				ForBonusPoints = isForBonus,
				// todo: Name, Id
			};
		}
	}
}

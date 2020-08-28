using System;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;
using CbCart.Repository.Entities;

namespace CbCart.Repository
{
	public class CbCartRepository : ICbCartRepository, IDisposable
	{
		// redis hash field name
		public const string CREATE_DATE = "cbcart_create_date";
		// redis hash field name
		public const string WEBHOOK = "webhook";
		// redis hash counter field name prefix
		public const string PRODUCT = "product";
		// redis hash field name
		public const string F_FOR_BONUS_POINTS = "ForBonusPoints";
		// redis hash field name
		public const string F_COST = "Cost";

		// redis set name
		private const string cbCarts = "cbcarts";
		// redis hash name prefix
		private const string cbCart = "cbcart";
		// redis hash name prefix
		private const string cbCartProduct = "cbcart_product";
		// redis set name
		private const string cbCartProducts = "cbcart_products";

		private ConnectionMultiplexer redis;
		private IDatabase db;

		public CbCartRepository(string redisUrl)
		{
			//Console.WriteLine("Creating repo " + redisUrl);
			redis = ConnectionMultiplexer.Connect(redisUrl);
			db = redis.GetDatabase();
		}

		public string CreateCart()
		{
			var newCartId = Guid.NewGuid().ToString();
			db.SetAdd(cbCarts, newCartId);
			db.HashSet($"{cbCart}:{newCartId}", CREATE_DATE, DateTime.UtcNow.ToString());

			return newCartId;
		}

		public long AddToCart(string cartId, string productId, int quantity)
		{
			CheckCartExists(cartId);
			CheckProductExists(productId);
			return db.HashIncrement($"{cbCart}:{cartId}", $"{PRODUCT}:{productId}", quantity);
		}

		public long RemoveFromCart(string cartId, string productId, int quantity)
		{
			CheckCartExists(cartId);
			CheckProductExists(productId);
			if (db.HashGet($"{cbCart}:{cartId}", $"{PRODUCT}:{productId}") == 0)
			{
				return 0;
			}

			return db.HashDecrement($"{cbCart}:{cartId}", $"{PRODUCT}:{productId}", quantity);
		}

		public bool SetWebHook(string cartId, string url)
		{
			CheckCartExists(cartId);
			return db.HashSet($"{cbCart}:{cartId}", WEBHOOK, url);
		}

		public Cart GetCart(string cartId)
		{
			CheckCartExists(cartId);
			return RedisEntryToEntityMapper.CreateCart(cartId, db.HashGetAll($"{cbCart}:{cartId}"));
		}

		public bool DeleteCart(string cartId)
		{
			CheckCartExists(cartId);
			var res = db.SetRemove(cbCarts, cartId);
			foreach (var productId in GetAllProducts())
			{
				db.HashDelete($"{cbCart}:{cartId}", $"{PRODUCT}:{productId}");
			}
			db.HashDelete($"{cbCart}:{cartId}", WEBHOOK);
			db.HashDelete($"{cbCart}:{cartId}", CREATE_DATE);

			return res;
		}

		public IEnumerable<string> GetAllCarts()
		{
			return db.SetMembers(cbCarts).Select(rv =>
			{
				return rv.ToString();
			});
		}

		private IEnumerable<string> GetAllProducts()
		{
			return db.SetMembers(cbCartProducts).Select(rv =>
			{
				return rv.ToString();
			});
		}

		public Product GetProduct(string productId)
		{
			var hashEntries = db.HashGetAll($"{cbCartProduct}:{productId}");
			return RedisEntryToEntityMapper.CreateProduct(hashEntries);
		}

		private bool CheckCartExists(string cartId)
		{
			if (db.SetContains(cbCarts, cartId))
				return true;

			throw new Exception("Cart not found!");
		}

		private bool CheckProductExists(string productId)
		{
			if (db.SetContains(cbCartProducts, productId))
				return true;

			throw new Exception("Product not found!");
		}

		public void Dispose()
		{
			if (redis != null)
			{
				redis.Dispose();
			}
			//Console.WriteLine("repo disposed");
		}
	}
}
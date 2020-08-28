using CbCart.Repository.Entities;
using System;
using System.Collections.Generic;

namespace CbCart.Repository
{
	public interface ICbCartRepository
	{
		IEnumerable<string> GetAllCarts();
		string CreateCart();
		Cart GetCart(string cartId);
		long AddToCart(string cartId, string productId, int quantity);
		long RemoveFromCart(string cartId, string productId, int quantity);
		bool SetWebHook(string cartId, string url);
		bool DeleteCart(string cartId);
		Product GetProduct(string productId);
	}
}
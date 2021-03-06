﻿using System;
using CbCart.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CbCartService.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private ICbCartRepository repo;
		
		public CartController(ICbCartRepository repo)
		{
			this.repo = repo;
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(500)]
		public IActionResult New()
		{
			var value = repo.CreateCart();
			return new OkObjectResult(value);
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public IActionResult Add([FromQuery] string cartId, [FromQuery] string productId, [FromQuery] int quantity)
		{
			try
			{
				if (quantity <= 0) {
					throw new Exception("Wrong quantity!");
				}
				var value = repo.AddToCart(cartId, productId, quantity);
				return new OkObjectResult(value);
			}
			catch (Exception e)
			{
				return new BadRequestObjectResult(e.Message);
			}
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public IActionResult Remove([FromQuery] string cartId, [FromQuery] string productId, [FromQuery] int quantity)
		{
			try
			{
				if (quantity <= 0)
				{
					throw new Exception("Wrong quantity!");
				}
				var value = repo.RemoveFromCart(cartId, productId, quantity);
				return new OkObjectResult(value);
			}
			catch (Exception e)
			{
				return new BadRequestObjectResult(e.Message);
			}
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public IActionResult SetWebHook([FromQuery] string cartId, [FromQuery] string url)
		{
			try
			{
				// just to validate URL
				var u = new Uri(url);
				var value = repo.SetWebHook(cartId, url);
				return new OkObjectResult(value);
			}
			catch (Exception e)
			{
				return new BadRequestObjectResult(e.Message);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace CbCart.Repository.Entities
{
	public class Cart
	{
		public string Id { get; set; }
		public string WebHook { get; set; }
		public IEnumerable<Tuple<string, int>> Positions { get; set; }
		public DateTime Created { get; set; }
	}
}

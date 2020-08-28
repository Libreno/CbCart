using System;
using System.Linq;
using System.Threading.Tasks;

namespace CbCart.Repository.Entities
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Cost { get; set; }
		public bool ForBonusPoints { get; set; }
	}
}
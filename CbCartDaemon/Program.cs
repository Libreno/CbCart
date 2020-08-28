using System;

namespace CbCart.Daemon
{
	class Program
	{
		static void Main(string[] args)
		{
			int daysCount = 0;
			if (args.Length > 0)
			{
				if (!int.TryParse(args[0], out daysCount))
				{
					Console.WriteLine($"Wrong parameter 'days count' value: {args[0]}, must be integer!");
					return;
				}
			} else
			{
				daysCount = 30;
			}
			var folder = ".\\";
			if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
			{
				folder = args[1];
			}
			var redisUrl = "127.0.0.1:6379";
			if (args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]))
			{
				redisUrl = args[2];
			}
			new CbDaemon(daysCount, folder, redisUrl).Run();
		}
	}
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace TweetSimulation
{
   public class Program
    {
      
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ITweet app = serviceProvider.GetService<TweetServices>();


            Console.WriteLine("Please enter Followers file name (press CTRL+Z to exit):");
            string followersFile  = Console.ReadLine();
            Console.WriteLine("Pleasde enter tweets from followers (press CTRL+Z to exit):");
            var TweetsFile = Console.ReadLine();
            var twetes = app.OutPutMessages(followersFile, TweetsFile);
            Console.WriteLine(twetes);

        }
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
            .AddTransient<TweetServices>();
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Reflection;

namespace TwitterSimulationTest
{
    [TestClass]
    public class TwitterSimulationTest
    {
        public readonly ITweet _tweet;
        public TwitterSimulationTest()
        {
            var logger = new Mock<ILogger<TweetServices>>().Object;
            _tweet = new TweetServices(logger);
        }
        [TestMethod]
        public void MyTwitterFeedIsNull_When_No_userFile_Provided()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tweet.txt");
            var mytweet = _tweet.MytweetFeeds("", fileName);
            Assert.AreEqual(mytweet, "");
        }
        [TestMethod]
        public void MyTwitterFeedIsNull_When_No_TweetFile_been_Provided()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.txt");
            var mytweet = _tweet.MytweetFeeds(fileName, "");
            Assert.AreEqual(mytweet, "");
        }
        [TestMethod]
        public void MyTwitterFeedIsNotNull_When_Both_TweetFile_And_FollloersFile_been_Provided()
        {
            string userFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.txt");
            string tweetFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tweet.txt");
            var mytweet = _tweet.MytweetFeeds(userFile, tweetFile);
            Assert.IsNotNull(mytweet);
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class TweetServices: ITweet
{
    ILogger _logger;
    public TweetServices(ILogger logger)
	{
        _logger = logger;
	}

    private List<Follower> GetFollowers(string fileName)
    {
        try { 
              var followers = new List<Follower>();
            using (StreamReader file = new StreamReader(fileName))
            {
                string line;
                // var follower = new Follower();
                while ((line = file.ReadLine()) != null)
                {
                    var follower = new Follower();
                    var lines = line.Split("follows");

                    var newDict = lines[1].Split(",");

                    if (followers != null && followers.Where(x => x.follower == lines[0]).Any())
                    {
                        followers.Where(x => x.follower.Trim() == lines[0].Trim()).FirstOrDefault().Followees.AddRange(newDict);
                    }
                    else
                    {
                        follower.follower = lines[0];
                        foreach (var word in newDict)
                        {
                            follower.Followees = new List<string>
                        {
                            word
                        };
                        }
                    }
                    if (follower != null && follower.follower != null && follower.Followees != null)
                    {
                        followers.Add(follower);
                    }
                }
                file.Close();

                return followers;
            }    
 
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while getting folloers from the file");
            return null;
        }
    }

    private List<Tweets> GetTweet(string fileName)
    {
        try
        {
            var tweetsMesages = new List<Tweets>();
            using (StreamReader file = new StreamReader(fileName))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    var lines = line.Split(">");
                    var tweets = new Tweets();

                    if (tweetsMesages != null && tweetsMesages.Where(x => x.tweetfollower == lines[0]).Any())
                    {
                        tweets.tweetfollower = lines[0];
                        tweetsMesages.Where(x => x.tweetfollower == lines[0]).FirstOrDefault().tweets.Add(lines[1]);
                    }
                    else
                    {
                        tweets.tweetfollower = lines[0];
                        tweets.tweets = new List<string>
                        {
                            lines[1]
                        };
                    }

                    if (tweets != null && tweets.tweetfollower != null && tweets.tweets != null)
                    {
                        tweetsMesages.Add(tweets);
                    }
                }
                file.Close();
            }
            return tweetsMesages;
        }
             
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while getting retrieving feeds from the timeLine from the file");
            return null;
        }
    }

    public string OutPutMessages(string userfile, string tweetfile)
    {
        StringBuilder str = new StringBuilder();
        var getuser = GetFollowers(userfile);
        var gettweet = GetTweet(tweetfile);

        foreach (var user in getuser)
        {
            //var userTweet = gettweet.Where(x => x.tweetfollower.Trim() == user.follower.Trim());
            if (gettweet.Where(x => x.tweetfollower.Trim() == user.follower.Trim()).Any())
            {
                str.Append(user.follower.Trim());
                str.AppendLine();

                foreach(var tweet in gettweet.Where(x => x.tweetfollower.Trim() == user.follower.Trim()).FirstOrDefault().tweets)
                {
                    str.Append("@" + user.follower + ":" + tweet);
                    str.AppendLine();
                }

                foreach (var followee in user.Followees)
                {
                    if (gettweet.Where(x => x.tweetfollower.Trim() == followee.Trim()).Any())
                    {
                        foreach (var tweet in gettweet.Where(x => x.tweetfollower.Trim() == followee.Trim()).FirstOrDefault().tweets)
                        {
                            str.Append("@" + followee + ":" + tweet);
                            str.AppendLine();
                        }
                    }
                    str.Append(followee.Trim());
                    str.AppendLine();
                }
            }

        }
        return str.ToString();
    }

}

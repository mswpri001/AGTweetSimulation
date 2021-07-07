using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class TweetServices: ITweet
{
    ILogger<TweetServices> _logger;
    public TweetServices(ILogger<TweetServices> logger)
	{
        _logger = logger;
	}
    /// <summary>
    /// This method retrieves followers/users from the user file
    /// </summary>
    /// <param name="fileName" "i.e. user.txt"></param>
    /// <returns></returns>
    private List<Follower> GetFollowers(string fileName)
    {
        try {
            _logger.LogInformation("Retrieving followers");

              var followers = new List<Follower>();
            if (fileName != "")
                {
                    using (StreamReader file = new StreamReader(fileName))
                    {
                        string line;
                        // var follower = new Follower();
                        while ((line = file.ReadLine()) != null)
                        {
                            var follower = new Follower();
                            var lines = line.Split(" follows ");

                            var newDict = lines[1].Split(", ");
                            var itemExist = followers.FirstOrDefault(x => x.follower == lines[0]);
                            var dict = followers.ToDictionary(x => x.follower);

                        if (followers != null && itemExist != null)
                        {
                            Follower found;
                            var listFollowee = itemExist.Followees.ToList();
                            var unioList = listFollowee.Union(newDict).ToList();
                            if (dict.TryGetValue(lines[0], out found)) found.Followees = unioList;

                            itemExist.Followees = unioList;
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
                                _logger.LogInformation("Successfully retrieved followers");
                            }
                        }
                        file.Close();                        
                        return followers;
                    }
                }
            _logger.LogError("File path not provided");
            return null;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while getting followers from the file");
            return null;
        }
    }
    /// <summary>
    /// This method retrieves Feeds from tweet file
    /// </summary>
    /// <param name="fileName""i.e. tweet.txt"></param>
    /// <returns></returns>
    private List<Tweets> GetTweet(string fileName)
    {
        try
        {
            _logger.LogInformation("Retrieving tweets");

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
                        _logger.LogInformation("Successfully retrieved tweets");
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

    public string MytweetFeeds(string userfile, string tweetfile)
    {
        StringBuilder str = new StringBuilder();
        var getUser = GetFollowers(userfile);
        var getTweet = GetTweet(tweetfile);
        var filteredUsers = FilterUsers(getUser);

        if (getUser != null)
        {
            
                if (getTweet != null)
                {
                    foreach (var filteruser in filteredUsers)
                    {
                    str.Append(filteruser);
                    str.AppendLine();

                    if (getTweet.Where(x => x.tweetfollower == filteruser).Any())
                        {                        
                            foreach (var tweet in getTweet?.Where(x => x.tweetfollower == filteruser).FirstOrDefault().tweets)
                            {
                                str.Append("@" + filteruser + ":" + tweet);
                                str.AppendLine();
                            }

                        var filterdUserForTweets = getUser.FirstOrDefault(x => x.follower == filteruser);

                            foreach (var followee in filterdUserForTweets.Followees)
                            {
                                if (getTweet.Where(x => x.tweetfollower == followee).Any())
                                {
                                    foreach (var tweet in getTweet.Where(x => x.tweetfollower == followee).FirstOrDefault().tweets)
                                    {
                                        str.Append("@" + followee + ":" + tweet);
                                        str.AppendLine();
                                    }
                                }                               
                            }
                        }
                    }
                }           

        }
        
        return str?.ToString();
    }

    private List<string> FilterUsers(List<Follower> filter)
    {
        var followers = filter?.Select(x => x.follower)?.ToList();
        var followees = filter?.SelectMany(x => x.Followees)?.ToList();
        var filtered = followers?.Union(followees)?.ToList();

        return (filtered)?.OrderBy(x => x?.Substring(0))?.ToList();
    }

}

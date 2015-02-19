using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CsQuery;
using SteamInfo.Domain;
using SteamInfo.Exceptions;

namespace SteamInfo
{
	public class SteamIdFinder : ISteamIdFinder
	{
		private DateTime _nextQueryAt;

		public SteamIdFinder()
		{
			_nextQueryAt = DateTime.Now;
		}

		// TODO: refactor
		public long? GetSteamIdByName(string name)
		{
			Debug.Print("\nFinding Steam ID for: {0}", name);

			if (string.IsNullOrWhiteSpace(name))
				return null;

			var player = Config.UseCache
				? GetFromCache(name)
				: GetFromWebRequest(name);

			return player.SteamId;
		}

		internal Player GetFromCache(string name)
		{
			Debug.Print("\tLooking in cache");

			// TODO: interface and query class
			var cache = new CacheContext();
			var player = cache.Players.SingleOrDefault(p => p.Name == name);

			if (player != null)
			{
				Debug.Print("\tFound in cache: {0}", player.SteamId);
				return player;
			}

			player = GetFromWebRequest(name);

			cache.Players.Add(player);
			cache.SaveChanges();

			return player;
		}

		internal Player GetFromWebRequest(string name)
		{
			const string urlMask = "http://steamcommunity.com/actions/Search?T=Account&K=%22{0}%22";

			EnsureWebRequestsAreThrottled();

			Debug.Print("\tSearching");
			var searchUrl = string.Format(urlMask, HttpUtility.UrlEncode(name));
			var dom = CQ.CreateFromUrl(searchUrl);
			var profileUrls = FindProfileUrls(dom);

			Debug.Print("\tProfile URL Count: {0}", profileUrls.Count);

			var steamId = GetFromProfileUrls(profileUrls);

			return new Player
			{
				Name = name,
				SteamId = steamId,
				QueryCount = profileUrls.Count,
				QueriedAt = DateTime.Now
			};
		}

		internal void EnsureWebRequestsAreThrottled()
		{
			const int secondsBetweenWebRequests = 4;
			const int sleepyTime = 500;

			var now = DateTime.Now;

			while (now < _nextQueryAt)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(sleepyTime));
				now = DateTime.Now;
				Debug.Print("\tSearching in: {0}", (_nextQueryAt - now).TotalSeconds);
			}

			_nextQueryAt = now.AddSeconds(secondsBetweenWebRequests);
		}

		internal long? GetFromProfileUrls(List<string> profileUrls)
		{
			if (profileUrls.Count != 1)
				return null;

			var url = profileUrls.Single();

			Debug.Print("\tProfile URL: {0}", url);
			
			const string urlMask = "http://steamcommunity.com/profiles/";

			if (!url.StartsWith(urlMask))
				return null; // TODO: resolve vanity URLs

			long id;
			if (!long.TryParse(url.Replace(urlMask, ""), out id))
				throw new HolyShitException("This URL be fricked up son! " + url);

			Debug.Print("\tSteam ID: {0}", id);

			return id;
		}

		internal List<string> FindProfileUrls(CQ dom)
		{
			return dom["div.resultItem a.linkTitle"]
				.Select(a => a["href"])
				.ToList();
		}
	}
}

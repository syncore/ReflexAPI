# ReflexAPI

ReflexAPI is a lightweight .NET (C#) back end web service that allows users to retrieve current [Reflex] server data from Valve Software's Steam Master Server. It runs as a standalone console application and does not require an ASP.NET setup. It is capable of returning the server information as JSON (default), XML, as well as in other formats.

This is the back end service that is currently providing data to my Reflex Server Browser at: http://reflex.syncore.org

Pull requests are welcome!


----------


### Tech
ReflexAPI incorporates the following open source technologies:

  - [ServiceStack] for the API framework
  - A lighter, modified version of [Stajs.BalloonicornHunter] as the Steam query library
  - [QuartzNET] for job scheduling
  - [Log4Net] for logging

### Version
1.0.0

### Installation

- Build in Visual Studio 2012 or higher.
	- Before attempting to build, enable the NuGet Package Restore for the ReflexApi solution, by right-clicking it and selecting *Enable NuGet Package Restore*.
- If running on Linux, install [Mono] and its dependencies.
- If you are proxying requests, I'd recommend installing [nginx] instead of the mono Apache module.
- By default, the API service will listen on http://*:29405/
	- To change this, launch the ReflexApi executable with an argument containing the interface's IP and port, for example: `ReflexApi.exe http://*:80/` to listen on all interfaces on port 80.

### Usage

 - **Retrieve full listing of servers**
	 - `GET /servers`
		 - This is used to retrieve the entire unfiltered list of global Reflex servers.
 - **Retrieve filtered list of servers**
	 - `GET /servers?filterName1=type1&filterName2=type2&filterNameN=typeN`
		 - This is used to further filter the master server list based on certain criteria such as whether the server has players, is running a certain map, or requires a password, etc. In Reflex's current state, some of these properties will be the same for all servers, but are included right now so they can be used when they are applicable to future Reflex builds; for example, the server's operating system type will vary once Linux dedicated binaries are available.
		 - Here are the possible filters. The format is: *data type | filter name. The filter name is case insensitive:*

		 - **boolean | *hasPassword***
			 - **true**: Show servers that are password protected.
			 - **false**: Show servers that are not password protected.
			 - `GET /servers?hasPassword=true`
		 - **boolean | *hasPlayers***
			 - **true**: Show servers with active players.
			 - **false**: Show only empty servers.
			 - `GET /servers?hasPlayers=true`
		 - **boolean | *hasVac***
		 	 - **true**: Show servers with Valve Anti Cheat enabled.
			 - **false**: Show servers without Valve Anti Cheat enabled.
			 - `GET /servers?hasVac=true`
		 - **boolean | *isNotFull***
		 	 - **true**: Show servers that are not full.
			 - **false**: Show both full and non-full servers.
			 - `GET /servers?isNotFull=true`
		 - **string | *countryCode***
		 	 - Show servers from a specified two letter country code.
			 - `GET /servers?countryCode=US`
		 - **string | *map***
		 	 - Show servers currently playing the specified map.
			 - `GET /servers?map=bdm3`
		 - **string | *os***
		 	 - Show servers currently running the specified operating system.
			 - `GET /servers?os=windows`
		 - **string | *version***
		 	 - Show servers currently running the specified Reflex build.
			 - `GET /servers?version=0.30.4`
		 - ***Note*: it is possible to chain filters together:**
		 	 - For example, to retrieve all Microsoft Windows servers running Reflex Build 0.30.4 in the United States on map cpm3 with active players:
			 - `GET /servers?os=windows&version=0.30.4&countryCode=US&map=cpm3&hasPlayers=true`
 - **Querying servers**
	 - `GET /queryserver?host=address1,address2,address3...address10&port=port1,port2,port3...port10`
		 - This is used to retrieve the most up-to-date, real-time information for up to ten (10) Reflex servers at a time.
			 - ***Notes:***
			 - When specifying the port, you must specify the Reflex server's Steam port, not the actual game server port. This is typically `port 25797`.
			 -  Server querying is purposefully limited to servers that exist in the current master list. If a server is alive at a given host:port but has not been indexed yet, try the request after the requisite 90 seconds have passed between index attempts.
				 - An endpoint URL can be configured that removes this master list restriction. For example, see the source files `Models/PrivateQueryRequest.cs` and `Services/PrivateQueryService.cs`
		 - **Example single server query**:
			 - `GET /queryserver?host=77.204.59.145&port=25797`
		 - **Example multiple server query of 3 servers**
			 - `GET /queryserver?host=77.204.59.145,reflex.mygameserver.com,24.163.49.111&port=25797,25797,25798`


### Help / Issues

I can be found under the name "syncore" on QuakeNet IRC - **irc.quakenet.org** - in the **#reflex** channel.
The preferable method of contact would be for you to open up an [issue] on Github.

### TODO

 - Implement additional features based on user requests.
 - Investigate where scalability improvements can be made once the full game is released and there are a lot more servers and a heavier load.


License
----
See LICENSE.md

[issue]:https://github.com/syncore/ReflexAPI/issues
[Mono]:http://www.mono-project.com/download/
[nginx]:http://www.nginx.com
[Reflex]:http://www.reflexfps.net
[ServiceStack]:https://servicestack.net
[Stajs.BalloonicornHunter]:https://github.com/stajs/Stajs.BalloonicornHunter
[HyperFastCGI]:https://github.com/xplicit/HyperFastCgi
[QuartzNET]:http://www.quartz-scheduler.net/
[Log4Net]:https://www.nuget.org/packages/log4net/

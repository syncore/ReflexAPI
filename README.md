# ReflexAPI

ReflexAPI is a .NET (C#) back-end web service that allows users to retrieve the current list of [Reflex] servers from Valve Software's Steam Master server. It is capable of returning the information as JSON (default), XML, as well as in other formats.

It features the following open source technologies:

  - [ServiceStack] for the API framework functionality
  - Lighter, modified version of [Stajs.BalloonicornHunter] as Steam query library
  - [QuartzNET] for job scheduling
  - [Log4Net] for logging

### Version
1.0.0

### Installation

- Build in Visual Studio 2012 or higher.
- If running on Linux, install [Mono] and its dependencies.
- If you are proxying requests, I'd recommend installing [nginx] instead of the mono Apache module.
- By default, the API will listen on http://*:29405/

### TODO

 - More features based on user requests
 - Possible scalability improvements once the full game is released and there are a lot more servers.

### Help / Issues

I can be found under the name "syncore" on QuakeNet IRC - irc.quakenet.org - in the #reflex channel.
Alternatively, open up an [issue] on Github.

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

This project exists to be a tool that I can use (or give to others, who need it) and as a learning exercise. Playing around in the world of webscraping is quite enjoyable, and I've used a handful of new tools on this project, and I plan to implement a bunch more. The end goal is to create the most robust and quick web scraper for Instagram that is out there. 

The version that I did using CasperJS/SlimerJS was quite quick, though hard to maintain. It had a number of issues with logins and the like, so it wasn't ideal. The current version made with Selenium in C# is much, much more maintainable and likely better overall, though it is a bit slow. I've tried to make it compatible with Chrome and FireFox, but FireFox is incredibly slow, and Chrome is very touchy. It likes to call it quits if the element isn't at the top of the DOM. 

I plan to use PuppeteerSharp for sure, anything beyond that is unplanned. I might branch into F# and Canopy too, but we'll see.

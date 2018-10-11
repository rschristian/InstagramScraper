This project exists as both a learning exercise and a method to go about data retrieval from Instagram. Whether is for the backup of data, or use in another project (client's goal was to create a collage from all of a user's uploads), this will allow quick download of all images on a profile and will eventually save all comments on the posts as well. 


Dependencies - Casperjs, Slimerjs

Casperjs is needed to parse ReactJS (as far as I know), and SlimerJS is used to capture media from srcsets, as PhantomJS (a dependency of Casper) will not be able to handle srcset media. SlimerJS therefore is used as the engine of choice.



Run (on Linux systems) with:
	csc instaScraper.cs -r:System.Windows.Forms.dll
	mono instaScraper.exe



Currently the log-in features are a bit touchy, due to anti-bot measures that instagram has put in place. Until that is sorted, I recommend not using the headless mode, and clicking the "log-in" button manually. This passes the anti-bot check.

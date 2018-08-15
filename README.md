This project exists as both a learning exercise and a method to go about data retrieval from Instagram. Whether is for the backup of data, or use in another project (client's goal was to create a collage from all of a user's uploads), this will allow quick download of all images on a profile and will eventually save all comments on the posts as well. 


Dependencies - Casperjs, Slimerjs

Casperjs is needed to parse ReactJS (as far as I know), and SlimerJS is used to capture media from srcsets, as PhantomJS (a dependency of Casper) will not be able to handle srcset media. SlimerJS therefore is used as the engine of choice.



Run (on Linux systems) with:
	casperjs --engine=slimerjs Master.js --targetAccount="(account name)" --retrieveText="(true/false)" --username="(your account username)" --password="(your account password)" --headless


For now, the script does need a valid login, but that will change in time when I add a more extensive options menu. The script, as it exists now, only offers the full suite of (finished) data retrieval options. Meaning, every working feature is enabled.

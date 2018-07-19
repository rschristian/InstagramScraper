This project exists as both a learning exercise and a method to go about data retrieval from Instagram. Whether is for the backup of data, or use in another project (client's goal was to create a collage from all of a user's uploads), this will allow quick download of all images on a profile and will eventually save all comments on the posts as well. 



Dependencies - Casperjs, Slimerjs


Run (on Linux systems) with:
	casperjs --engine=slimerjs Master.js --targetAccount="(account name)" --retrieveText="(true/false)" --headless

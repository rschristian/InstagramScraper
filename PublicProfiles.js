// noinspection JSUnusedGlobalSymbols
// noinspection JSUnusedGlobalSymbols
const casper = require("casper").create({
  viewportSize: {width: 19200, height:480},
  pageSettings: {
    loadImages: false,
    loadPlugins: false
  }
});

//Global variables
const targetAccount = casper.cli.get('targetAccount'),
      retrieveText = casper.cli.get('retrieveText'),
      t0 = performance.now(),
      path = "/home/ryan/Pictures/" + targetAccount + "/",
      fs = require('fs');


let t1,
    t2,
    t3,
    t4,
    dirtySrcSets = [],
    dirtyImgNames = [],
    commentsUsers = [],
    comments = [],
    pictsInSet = 1,
    post = {},
    storyDone = false,
    casperDone = false;

//HTML Tags
const
    //Page content container
    pageContentClass = '.v9tJq',

    //Page privacy status
    pagePrivateClass = '.QlxVY',

    //Class that each post is contained under
    storyClass = 'div.RR-M-',

    //Story existence status
    profileStoryClass = '.h5uC0',

    //Each post has this class assigned to it
    postsClass = 'div._bz0w a',

    //Profile pic (both in post and on main page) has this class
    profilePictureClass = "._6q-tv",

    //Decides if post is single item or an album
    chevronRootClass = "._97aPb ",

    //If post item is a video, this will exist
    videoSrcClass = ".tWeCl",

    //This class will exist for each post item (thumbnails)
    imageSrcClass = ".FFVAD",


    //WIP
    commentsUserClass = ".FPmhX",
    commentsTextClass = "div.C4VMK span";


//Gets the links for the image to then enter the first one
function enterPost(sel) {
    const links = document.querySelectorAll(sel);
    return Array.prototype.map.call(links, function(e) {
      return e.getAttribute('href')
    });
}


//Handles retrieving the profile picture and getting the proper name for it
function profilePicture(arrayURL, arrayNames) {
    casper.waitForSelector(profilePictureClass, function() {
        arrayURL.push(casper.evaluate(getProfilePic, profilePictureClass));
        arrayNames.push(todaysDate() + " profile");
  })
}

//Retrieves the profile pic from the page
function getProfilePic(sel) {
    const scripts = document.querySelectorAll(sel);
    return Array.prototype.map.call(scripts, function (e) {
        return e.getAttribute("src");
    });
}


//Recursive function that first checks if there is a right chevron.
//If so, it adds the img srcset to an array, clicks on that chevron,
//then checks again. Once there is only a left chevron, it returns the
//entire array of srcsets for that post. video class: _l6uaz img class: _2di5p
function retrievePostDate(arrayURL, arrayNames) {
    casper.waitForSelector(chevronRootClass, function(){
        if (casper.exists(".coreSpriteRightChevron")) {
            pictsInSet++;
            if (casper.exists(videoSrcClass)) {
                const vidURL = casper.evaluate(getVideoSrc, videoSrcClass).toString().split(',');
                for (let i = 0; i<vidURL.length; i++) {
                    arrayURL.push(vidURL[i]);
                }
            }
            if (casper.exists(imageSrcClass)) {
                const partsOfStr = casper.evaluate(getImageSrc, imageSrcClass).toString().split(',');
                for (let i =0; i<partsOfStr.length; i++) {
                    if (partsOfStr[i].includes("1080w")) {
                        arrayURL.push(partsOfStr[i].toString().slice(0,-6));
                    }
                }
            }
            casper.click(".coreSpriteRightChevron");
            retrievePostDate(arrayURL, arrayNames);
        } else if (!casper.exists(".coreSpriteRightChevron")) {
            if (casper.exists(videoSrcClass)) {
                const vidURL = casper.evaluate(getVideoSrc, videoSrcClass).toString().split(',');
                for (let i = 0; i<vidURL.length; i++) {
                    arrayURL.push(vidURL[i]);
                }
            }
            if (casper.exists(imageSrcClass)) {
                const partsOfStr = casper.evaluate(getImageSrc, imageSrcClass).toString().split(',');
                for (let i =0; i<partsOfStr.length; i++) {
                    if (partsOfStr[i].includes("1080w")) {
                        arrayURL.push(partsOfStr[i].toString().slice(0,-6));
                    }
                }
            }
            for (pictsInSet; pictsInSet>0; pictsInSet--) {
                arrayNames.push(refineTimeStamp() + " " + pictsInSet);
            }
            if (casper.exists(".coreSpriteRightPaginationArrow")){
                casper.click(".coreSpriteRightPaginationArrow");
                pictsInSet = 1;
                retrievePostDate(arrayURL, arrayNames);
            } else {
                console.log("Finished collecting all post data");
                casperDone = true;
                return arrayURL, arrayNames;
            }
        }
    })
}

function retrievePostTextData(arrayURL, arrayNames) {
    casper.waitForSelector(chevronRootClass, function(){
        if (casper.exists(".coreSpriteRightChevron")) {
            pictsInSet++;
            if (casper.exists(videoSrcClass)) {
                const vidURL = casper.evaluate(getVideoSrc, videoSrcClass).toString().split(',');
                for (let i = 0; i<vidURL.length; i++) {
                    arrayURL.push(vidURL[i]);
                }
            }
            if (casper.exists(imageSrcClass)) {
                const partsOfStr = casper.evaluate(getImageSrc, imageSrcClass).toString().split(',');
                for (let i =0; i<partsOfStr.length; i++) {
                    if (partsOfStr[i].includes("1080w")) {
                        arrayURL.push(partsOfStr[i].toString().slice(0,-6));
                    }
                }
            }
            casper.click(".coreSpriteRightChevron");
            retrievePostTextData(arrayURL, arrayNames);
        } else if (!casper.exists(".coreSpriteRightChevron")) {
            if (casper.exists(videoSrcClass)) {
                const vidURL = casper.evaluate(getVideoSrc, videoSrcClass).toString().split(',');
                for (let i = 0; i<vidURL.length; i++) {
                    arrayURL.push(vidURL[i]);
                }
            }
            if (casper.exists(imageSrcClass)) {
                const partsOfStr = casper.evaluate(getImageSrc, imageSrcClass).toString().split(',');
                for (let i =0; i<partsOfStr.length; i++) {
                    if (partsOfStr[i].includes("1080w")) {
                        arrayURL.push(partsOfStr[i].toString().slice(0,-6));
                    }
                }
            }
            for (pictsInSet; pictsInSet>0; pictsInSet--) {
                arrayNames.push(refineTimeStamp() + " " + pictsInSet);
            }

            let userList = casper.evaluate(getUser, commentsUserClass);
            userList.shift();
            commentsUsers.push(userList);
            // console.log(userList);
            // console.log("UserList Length: " + userList.length);
            let postComments = casper.evaluate(getComments, commentsTextClass);
            // console.log(postComments);
            // console.log("Post Comments Length: " + postComments.length);
            comments.push(postComments);

            // for (let i = 0; i<postComments.length; i++){
            //     console.log(postComments[i]);
            // }

            if (casper.exists(".coreSpriteRightPaginationArrow")){
                casper.click(".coreSpriteRightPaginationArrow");
                pictsInSet = 1;
                retrievePostTextData(arrayURL, arrayNames);
            } else {
                console.log("Done");
                casperDone = true;
                return arrayURL, arrayNames;
            }
        }
    })
}


function todaysDate() {
    let today = new Date();
    let dd = today.getDate();
    let  mm = today.getMonth() + 1;
    const yyyy = today.getFullYear();

    if(dd<10) {
        dd = '0'+dd
    }

    if(mm<10) {
        mm = '0'+mm
    }

    today = yyyy + "-" + mm + "-" + dd;
    return today;
}

//Refines the the time stamp into a usable name for a file
function refineTimeStamp() {
    let timeStamp = String(casper.evaluate(getTime));
    const indexOfT = timeStamp.indexOf('T');
    timeStamp = timeStamp.substr(0,indexOfT) + ' ' + timeStamp.substr(indexOfT+1);
    timeStamp = timeStamp.slice(0, -5);
    return timeStamp;
}

//Gets the time to name the pictures with
function getTime() {
    const scripts = document.querySelectorAll('time[datetime]');
    return Array.prototype.map.call(scripts, function (e) {
        return e.getAttribute('datetime');
    });
}


//Gets the image srcsets from the page
function getVideoSrc(sel) {
    const scripts = document.querySelectorAll(sel);
    return Array.prototype.map.call(scripts, function (e) {
        return e.getAttribute("src");
    });
}

function getImageSrc(sel) {
    const scripts = document.querySelectorAll(sel);
    return Array.prototype.map.call(scripts, function (e) {
        return e.getAttribute("srcset");
    });
}


function getUser(sel) {
    const scripts = document.querySelectorAll(sel);
    return Array.prototype.map.call(scripts, function (e) {
        return e.getAttribute("title");
    });
}

function getComments(sel) {
    const scripts = document.querySelectorAll(sel);
    return Array.prototype.map.call(scripts, function (e) {
        return e.textContent;
    });
}


function cleanDataSets(a) {
    let seen = {};
    return a.filter(function (item) {
        return seen.hasOwnProperty(item) ? false : (seen[item] = true);
    });
}




casper.start('https://www.instagram.com/' + targetAccount + '/'
).waitForSelector(pageContentClass, function() {
    if (casper.exists(pagePrivateClass)) {
      console.log("Error: Account is private and you are not logged in. " +
          "You will need to log in with an account that has permission " +
          "to view this user's page." + "\n");
      casper.exit();
    } else {
      console.log("Account is accessible");
    }
}).waitForSelector(storyClass, function() {
    if (casper.exists(profileStoryClass)) {
        console.log("Profile has a story, but without log in details, we can't " +
            "access it.");
        storyDone = true;
    } else {
        console.log("User does not have a story.");
        storyDone = true;
    }
}).waitFor(function check(){
    return storyDone;
}).then(function() {
    t1 = performance.now();
    let returnHref = this.evaluate(enterPost, postsClass);
    returnHref.length = 1;
    casper.click("a[href^='" + returnHref + "']");
}).then(function() {
    profilePicture(dirtySrcSets, dirtyImgNames);
}).then(function() {
    console.log("Retrieving media links");
    if (retrieveText !== true) {
        retrievePostDate(dirtySrcSets, dirtyImgNames);
    } else {
        console.log("Retrieving Text");
        retrievePostTextData(dirtySrcSets, dirtyImgNames);
    }
}).waitFor(function check(){
    t2 = performance.now();
    return casperDone;
}).then(function() {
    const finalisedLinks = cleanDataSets(dirtySrcSets);
    console.log("Number of Links: " + finalisedLinks.length);
    //for (let i =0; i<finalisedLinks.length; i++) {
        //console.log(i + "; " + finalisedLinks[i]);
    //}
    const finalisedNames = cleanDataSets(dirtyImgNames);
    console.log("Number of Names: " + finalisedNames.length);
    //for (let i =0; i<finalisedNames.length; i++) {
        //console.log(i + "; " + finalisedNames[i]);
    //}
    const finalisedCommentsUsers = cleanDataSets(commentsUsers);
    console.log("Number of posts with comments: " + finalisedCommentsUsers.length);
    for (let i =0; i<finalisedCommentsUsers.length; i++) {
    console.log(i + "; " + finalisedCommentsUsers[i]);
    }
    const finalisedComments = cleanDataSets(comments);
    console.log("Number of posts with comments: " + finalisedComments.length);
    console.log("Downloading....");
    t3 = performance.now();
    for (let i = 0; i<finalisedLinks.length; i++) {
        if (!(fs.exists(path + finalisedNames[i] + ".mp4") || fs.exists(path + finalisedNames[i] + ".jpeg"))) {
            if (String(finalisedLinks[i]).indexOf("mp4") > 0) {
                casper.download(finalisedLinks[i], path + finalisedNames[i] + ".mp4");
            } else {
                casper.download(finalisedLinks[i], path + finalisedNames[i] + ".jpeg");
            }
        }
    }

    // post['7-14-2018'] = ["G:happy birthday toddy!! #dadeo, C:ll,G:yy"];
    // post['7-15-2018'] = ["G: Yoinks Scoob, C: Heylo"];
    // for (let x in post) {
    //     console.log(x);
    //     let values = post[x];
    //     for (let y in values) {
    //         console.log(values[y]);
    //     }
    // }

    t4 = performance.now();
    console.log("Time to load page, login, and retrieve story: " + (t1-t0));
    console.log("Time to retrieve all media links: " + (t2-t1));
    console.log("Time per post: " + (t2-t1)/(finalisedNames.length -1));
    console.log("Time to clean arrays: " + (t3-t2));
    console.log("Time to download: " + (t4-t3));
    console.log("Total time taken: " + (t4-t0) + "\n");

});

casper.run();

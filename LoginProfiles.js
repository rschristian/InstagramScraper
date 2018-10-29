// noinspection JSUnusedGlobalSymbols
// noinspection JSUnusedGlobalSymbols
const casper = require("casper").create({
  viewportSize: {width: 19200, height:480},
  waitTimeout: 100000,
  stepTimeout: 105000,
  onWaitTimeout: function() {
    console.log("Wait timed out");
  },
  onStepTimeout: function() {
    console.log("Step timed out");
  },
  pageSettings: {
    loadImages: false,
    loadPlugins: false
  }
});

//Global variables
const targetAccount = casper.cli.get('targetAccount'),
      retrieveText = casper.cli.get('retrieveText'),
      username = casper.cli.get('username'),
      password = casper.cli.get('password'),
      captureStory = casper.cli.get('captureStory'),
      t0 = performance.now(),
      path = "/home/ryan/Pictures/" + targetAccount + "/",
      fs = require('fs');


let t1,
    t2,
    t3,
    t4,
    dirtySrcSets = [],
    dirtyImgNames = [],
    logInSuccess = false,
    comments = [],
    profileText = [],
    pictsInStory = [],
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

    //Class for nav chevron, ensuring necessary classes have been loaded
    storyRootClass = '.ow3u_',

    //If story is a video, this class exists
    storyVideoSrcClass = '.OFkrO source',

    //This class will exist no matter what (video thumbnail)
    storyImageSrcClass = '._7NpAS',

    //Each post has this class assigned to it
    postsClass = 'div._bz0w a',

    //Profile pic (both in post and on main page) has this class
    profilePictureClass = "._6q-tv",

    //Dictates when post is single item or an album
    chevronRootClass = "._97aPb ",

    //If post item is a video, this will exist
    videoSrcClass = ".tWeCl",

    //This class will exist for each post item
    imageSrcClass = ".FFVAD",


    //WIP
    commentsUserClass = ".FPmhX",
    commentsTextClass = ".gElp9";


//Logs in to Instagram (necessary to view posts or private profiles)
function logIn() {
    casper.sendKeys('input[name=username]', username);
    casper.sendKeys('input[name=password]', password);
    casper.wait(1500, function() {
        casper.click('.oF4XW');
    });
    if (!casper.exists('#slfErrorAlert')) {
        logInSuccess = true;
    }
}

//Retrieves the story items from the profile
function storyCapture(arrayURL, arrayNames) {
    casper.wait(500, function(){
        if (casper.exists(storyRootClass)) {
            pictsInStory++;
            if (casper.exists(storyVideoSrcClass)) {
                const vidURL = casper.evaluate(getVideoSrc, storyVideoSrcClass).toString().split(',');
                arrayURL.push(vidURL[0]);
            } else {
                const partsOfStr = casper.evaluate(getVideoSrc, storyImageSrcClass).toString().split(',');
                arrayURL.push(partsOfStr[partsOfStr.length-1]);
            }
            casper.click(storyRootClass);
            storyCapture(arrayURL, arrayNames);
        } else {
            for (pictsInStory; pictsInStory>0; pictsInStory--) {
                arrayNames.push(todaysDate() + " story " + pictsInStory);
            }
            storyDone = true;
            return arrayURL, arrayNames;
        }
    })

}

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
function checkAndGrab(arrayURL, arrayNames) {
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
            checkAndGrab(arrayURL, arrayNames);
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
                checkAndGrab(arrayURL, arrayNames);
            } else {
                console.log("Finished collecting all post data");
                casperDone = true;
                return arrayURL, arrayNames;
            }
        }
    })
}

// function checkGrabText(arrayURL, arrayNames) {
//     casper.waitForSelector(chevronRootClass, function(){
//         if (casper.exists(".coreSpriteRightChevron")) {
//             pictsInSet++;
//             const vidURL = casper.evaluate(getMediaSrc, videoSrcClass);
//             if (vidURL.length > 0) {
//                 arrayURL.push(vidURL);
//             } else {
//                 const partsOfStr = casper.evaluate(getMediaSrc, imageSrcClass).toString().split(',');
//                 arrayURL.push(partsOfStr[partsOfStr.length-1]);
//             }
//             casper.click(".coreSpriteRightChevron");
//             checkGrabText(arrayURL, arrayNames);
//         } else if (!casper.exists(".coreSpriteRightChevron")) {
//             const vidURL = casper.evaluate(getMediaSrc, videoSrcClass);
//             if (vidURL.length > 0) {
//                 arrayURL.push(vidURL);
//             } else {
//                 const partsOfStr = casper.evaluate(getMediaSrc, imageSrcClass).toString().split(',');
//                 arrayURL.push(partsOfStr[partsOfStr.length-1]);
//             }
//             for (pictsInSet; pictsInSet>0; pictsInSet--) {
//                 arrayNames.push(refineTimeStamp() + " " + pictsInSet);
//             }
//             let usrList = casper.evaluate(getUser, commentsUserClass);
//             comments.push(usrList.splice(0,1));
//             console.log(usrList);
//             let postComments = casper.evaluate(getComments, commentsTextClass);
//             console.log(postComments);
//             // for (let i = 0; i<usrList.length; i++){
//             //     console.log(usrList[i]);
//             // }
//             if (casper.exists(".coreSpriteRightPaginationArrow")){
//                 casper.click(".coreSpriteRightPaginationArrow");
//                 pictsInSet = 1;
//                 checkGrabText(arrayURL, arrayNames);
//             } else {
//                 console.log("Done");
//                 casperDone = true;
//                 return arrayURL, arrayNames;
//             }
//         }
//     })
// }

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

//Gets the time to name the pictures with
function getTime() {
    const scripts = document.querySelectorAll('time[datetime]');
    return Array.prototype.map.call(scripts, function (e) {
        return e.getAttribute('datetime');
    });
}

//Refines the the time stamp into a usable name for a file
function refineTimeStamp() {
    let timeStamp = String(casper.evaluate(getTime));
    const indexOfT = timeStamp.indexOf('T');
    timeStamp = timeStamp.substr(0,indexOfT) + ' ' + timeStamp.substr(indexOfT+1);
    timeStamp = timeStamp.slice(0, -5);
    return timeStamp;
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
        return e.getHTML("span");
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

function cleanDataSets(a) {
    let seen = {};
    return a.filter(function (item) {
        return seen.hasOwnProperty(item) ? false : (seen[item] = true);
    });
}




casper.start('https://www.instagram.com/' + targetAccount + '/'
).waitForSelector(pageContentClass, function() {
    // phantom.addCookie({
    //     'name': "sessionid",
    //     'value': "IGSC59e64fbfd66817fdb37c7ffb7804868e2fdcf9273cff626c7b8efda22a305454%3AHSzRvf1SM5T0p3Ii3nUmHV0P0BkCLN4T%3A%7B%22_auth_user_id%22%3A1754178657%2C%22_auth_user_backend%22%3A%22accounts.backends.CaseInsensitiveModelBackend%22%2C%22_auth_user_hash%22%3A%22%22%2C%22_platform%22%3A4%2C%22_token_ver%22%3A2%2C%22_token%22%3A%221754178657%3AZ6PUfbQBaFXicYmJsMZXKjdSW4A0ClwY%3Ad72ccc9b1fad789d0fc03b894d70469a954b77c41b271c9a141d6aca345f5760%22%2C%22last_refreshed%22%3A1539550276.0981733799%7D",
    //     'domain': ".instagram.com"
    // });
    if (casper.exists(pagePrivateClass)) {
        console.log("Error: Account is private and user does not have access. " +
            "You will need to request to follow this user, or log in " +
            "with a different account." + "\n");
        casper.exit();
    } else {
      console.log("Account is accessible");
    }
}).waitForSelector(storyClass, function() {
    casper.wait(500, function() {
        if (casper.exists(profileStoryClass) && captureStory === true) {
            console.log("Profile has a story and it is being downloaded.");
            casper.click(profileStoryClass);
            storyCapture(dirtySrcSets, dirtyImgNames);
        } else if (casper.exists(profileStoryClass) && !captureStory === true) {
            console.log("Ignoring the user's story.");
            storyDone = true;
        } else if (casper.exists(profileStoryClass) && username == null) {
            console.log("Profile has a story, but without log in details, we can't " +
                "access it.");
            storyDone = true;
        } else {
            console.log("User does not have a story. Moving on.");
            storyDone = true;
        }
    });
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
    console.log("Retrieving all media links");
    if (retrieveText !== true) {
        checkAndGrab(dirtySrcSets, dirtyImgNames);
    } else {
        console.log("Retrieving Text");
        checkGrabText(dirtySrcSets, dirtyImgNames);
    }
}).waitFor(function check(){
    t2 = performance.now();
    return casperDone;
}).then(function() {
    const finalisedLinks = cleanDataSets(dirtySrcSets);
    // console.log("Number of Links: " + finalisedLinks.length);
    //for (let i =0; i<finalisedLinks.length; i++) {
        //console.log(i + "; " + finalisedLinks[i]);
    //}
    const finalisedNames = cleanDataSets(dirtyImgNames);
    // console.log("Number of Names: " + finalisedNames.length);
    //for (let i =0; i<finalisedNames.length; i++) {
        //console.log(i + "; " + finalisedNames[i]);
    //}
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
    post['7-14-2018'] = ["G:happy birthday toddy!! #dadeo, C:ll,G:yy"];
    post['7-15-2018'] = ["G: Yoinks Scoob, C: Heylo"];
    for (let x in post) {
        //console.log(x);
        let values = post[x];
        for (let y in values) {
            //console.log(values[y]);
        }
    }
    t4 = performance.now();
    console.log("Time to load page, login, and retrieve story: " + (t1-t0));
    console.log("Time to retrieve all media links: " + (t2-t1));
    console.log("Time per post: " + (t2-t1)/(finalisedNames.length -1));
    console.log("Time to clean arrays: " + (t3-t2));
    console.log("Time to download: " + (t4-t3));
    console.log("Total time taken: " + (t4-t0) + "\n");

});

casper.run();

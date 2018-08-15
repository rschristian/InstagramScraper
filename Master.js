const casper = require("casper").create({
  viewportSize: {width: 1920, height:480},
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
      t0 = performance.now();
let t1,
    t2,
    t3,
    t4,
    dirtySrcSets = [],
    finalisedLinks = [],
    dirtyImgNames = [],
    finalisedNames = [],
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
    casper.click('._5f5mN');
}

//Retrieves the story items from the profile
function storyCapture(arrayURL, arrayNames) {
    casper.wait(500, function(){
        if (casper.exists(storyRootClass)) {
            pictsInStory++;
            if (casper.exists(storyVideoSrcClass)) {
                const vidURL = casper.evaluate(getMediaSrc, storyVideoSrcClass).toString().split(',');
                arrayURL.push(vidURL[0]);
            } else {
                const partsOfStr = casper.evaluate(getMediaSrc, storyImageSrcClass).toString().split(',');
                arrayURL.push(partsOfStr[partsOfStr.length-1]);
            }
            casper.click(storyChevronClass);
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
        console.log("Profile pic name: " + todaysDate());
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
            const vidURL = casper.evaluate(getMediaSrc, videoSrcClass);
            if (vidURL.length > 0) {
                arrayURL.push(vidURL);
            } else {
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
            const vidURL = casper.evaluate(getMediaSrc, videoSrcClass);
            if (vidURL.length > 0) {
                arrayURL.push(vidURL);
            } else {
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
                console.log("Done");
                casperDone = true;
                return arrayURL, arrayNames;
            }
        }
    })
}

function checkGrabText(arrayURL, arrayNames) {
    casper.waitForSelector(chevronRootClass, function(){
        if (casper.exists(".coreSpriteRightChevron")) {
            pictsInSet++;
            const vidURL = casper.evaluate(getMediaSrc, videoSrcClass);
            if (vidURL.length > 0) {
                arrayURL.push(vidURL);
            } else {
                const partsOfStr = casper.evaluate(getMediaSrc, imageSrcClass).toString().split(',');
                arrayURL.push(partsOfStr[partsOfStr.length-1]);
            }
            casper.click(".coreSpriteRightChevron");
            checkGrabText(arrayURL, arrayNames);
        } else if (!casper.exists(".coreSpriteRightChevron")) {
            const vidURL = casper.evaluate(getMediaSrc, videoSrcClass);
            if (vidURL.length > 0) {
                arrayURL.push(vidURL);
            } else {
                const partsOfStr = casper.evaluate(getMediaSrc, imageSrcClass).toString().split(',');
                arrayURL.push(partsOfStr[partsOfStr.length-1]);
            }
            for (pictsInSet; pictsInSet>0; pictsInSet--) {
                arrayNames.push(refineTimeStamp() + " " + pictsInSet);
            }
            let usrList = casper.evaluate(getUser, commentsUserClass);
            comments.push(usrList.splice(0,1));
            console.log(usrList);
            let postComments = casper.evaluate(getComments, commentsTextClass);
            console.log(postComments);
            // for (let i = 0; i<usrList.length; i++){
            //     console.log(usrList[i]);
            // }
            if (casper.exists(".coreSpriteRightPaginationArrow")){
                casper.click(".coreSpriteRightPaginationArrow");
                pictsInSet = 1;
                checkGrabText(arrayURL, arrayNames);
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
function getMediaSrc(sel) {
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

function cleanSrcSets(a) {
    let seen = {};
    finalisedLinks = a.filter(function(item) {
        return seen.hasOwnProperty(item) ? false : (seen[item] = true);
    });
}

function CleanImgNames(a) {
    let seen = {};
    finalisedNames = a.filter(function(item) {
        return seen.hasOwnProperty(item) ? false : (seen[item] = true);
    });
}




casper.start('https://www.instagram.com/accounts/login/'
).waitForSelector('.-MzZI', function() {
    logIn();
}).then(function() {
    casper.wait(500, function(){casper.thenOpen('https://www.instagram.com/'
        + targetAccount + '/')});
}).waitForSelector(pageContentClass, function() {
    if (casper.exists(pagePrivateClass)) {
      console.log("Account is private");
    } else {
      console.log("Account is public");
    }
}).waitForSelector(storyClass, function() {
    casper.wait(500, function() {
        if (casper.exists(profileStoryClass)) {
            console.log("Profile has a story");
            casper.click(profileStoryClass);
            storyCapture(dirtySrcSets, dirtyImgNames);
        } else {
            console.log("User does not have a story. Moving on.")
            storyDone = true;
        }
    });
}).waitFor(function check(){
    console.log('Finished Story');
    return storyDone;
}).then(function() {
    t1 = performance.now();
    let returnHref = this.evaluate(enterPost, postsClass);
    returnHref.length = 1;
    casper.click("a[href^='" + returnHref + "']");
}).then(function() {
    profilePicture(dirtySrcSets, dirtyImgNames);
}).then(function() {
    console.log("Entering posts grab");
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
    cleanSrcSets(dirtySrcSets);
    console.log("Number of Links: " + finalisedLinks.length);
    CleanImgNames(dirtyImgNames);
    console.log("Number of Names: " + finalisedNames.length);
    console.log("Downloading....");
    t3 = performance.now();
    for (let i = 0; i<finalisedLinks.length; i++) {
      if (String(finalisedLinks[i]).indexOf("mp4") > 0) {
        casper.download(finalisedLinks[i], "/home/ryan/Pictures/" + targetAccount + "/" + finalisedNames[i] + ".mp4");
      } else {
        casper.download(finalisedLinks[i], "/home/ryan/Pictures/" + targetAccount + "/" + finalisedNames[i] + ".jpeg");
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
    console.log("Time to load page: " + (t1-t0));
    console.log("Time to grab all srcsets: " + (t2-t1));
    console.log("Time per post: " + (t2-t1)/finalisedNames.length);
    console.log("Time to clean arrays: " + (t3-t2));
    console.log("Time to download: " + (t4-t3));
    console.log("Total time taken: " + (t4-t0));

});

casper.run();

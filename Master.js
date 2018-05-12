var casper = require("casper").create({
  viewportSize: {width: 1920, height:5},
  waitTimeout: 60000,
  stepTimeout: 80000,
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

var targetAccount = casper.cli.get('targetAccount');
var t0 = performance.now();
var t1;
var t2;
var t3;
var t4;
var dirtySrcSets = [];
var finalisedLinks = [];
var imgNames = [];
var finalisedNames = [];
var pictsInSet = 1;
var casperDone = false;



//Gets the mass of post hrefs that are sent to Post.js
function enterPost() {
  var links = document.querySelectorAll('div._mck9w a');
    return Array.prototype.map.call(links, function(e) {
        return e.getAttribute('href')
    });
}

//Gets the profile picture
function profilePicture(arrayURL, arrayNames) {
  casper.waitForSelector('._rewi8', function() {
    arrayURL.push(casper.evaluate(getProfilePic));
    console.log("Profile pic name: " + todaysDate());
    arrayNames.push(todaysDate());
  })
}

//Recursive function that first checks if there is a right chevron.
//If so, it adds the img srcset to an array, clicks on that chevron,
//then checks again. Once there is only a left chevron, it returns the
//entire array of srcsets for that post. video class: _l6uaz img class: _2di5p
function checkAndGrab(arrayURL, arrayNames) {
  casper.waitForSelector('._sxolz', function(){
    if (casper.exists(".coreSpriteRightChevron")) {
      pictsInSet++;
      var vidURL = casper.evaluate(getVidSrc)
      if (vidURL.length > 0) {
        arrayURL.push(vidURL);
      } else {
        var partsOfStr = casper.evaluate(getImgSrc).toString().split(',');
        arrayURL.push(partsOfStr[partsOfStr.length-1]);
      }
      casper.click(".coreSpriteRightChevron");
      checkAndGrab(arrayURL, arrayNames);
    } else if (!casper.exists(".coreSpriteRightChevron")) {
      var vidURL = casper.evaluate(getVidSrc)
      if (vidURL.length > 0) {
        arrayURL.push(vidURL);
      } else {
        var partsOfStr = casper.evaluate(getImgSrc).toString().split(',');
        arrayURL.push(partsOfStr[partsOfStr.length-1]);
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

function getProfilePic() {
  scripts = document.querySelectorAll("._rewi8");
  return Array.prototype.map.call(scripts, function (e) {
      return e.getAttribute("src");
  });
}

function todaysDate() {
  var today = new Date();
  var dd = today.getDate();
  var mm = today.getMonth() + 1;
  var yyyy = today.getFullYear();

  if(dd<10) {
    dd = '0'+dd
  }

  if(mm<10) {
      mm = '0'+mm
  }

  today = yyyy + "-" + mm + "-" + dd;
  var name = today + " profile";
  return name;
}

//Gets the time to name the pictures with
function getTime() {
  var scripts = document.querySelectorAll('time[datetime]');
  return Array.prototype.map.call(scripts, function (e) {
      return e.getAttribute('datetime');
  });
}

//Refines the the time stamp into a usable name for a file
function refineTimeStamp() {
  var timeStamp = String(casper.evaluate(getTime));
  var indexOfT = timeStamp.indexOf('T');
  timeStamp = timeStamp.substr(0,indexOfT) + ' ' + timeStamp.substr(indexOfT+1);
  timeStamp = timeStamp.slice(0, -5);
  return timeStamp;
}

//Gets the image srcsets from the page
function getImgSrc() {
  scripts = document.querySelectorAll("._2di5p");
  return Array.prototype.map.call(scripts, function (e) {
      return e.getAttribute("src");
  });
}

//Get/check video
function getVidSrc() {
  scripts = document.querySelectorAll("._l6uaz");
  return Array.prototype.map.call(scripts, function (e) {
      return e.getAttribute("src");
  });
}

function uniq(a) {
  seen = {};
  finalisedLinks = a.filter(function(item) {
      return seen.hasOwnProperty(item) ? false : (seen[item] = true);
  });
}

function uniqNames(a) {
  seen = {};
  finalisedNames = a.filter(function(item) {
      return seen.hasOwnProperty(item) ? false : (seen[item] = true);
  });
}




//Java is weird when executing commands, this shows address in case it gets wonky
// console.log("https://www.instagram.com/" + targetAccount +"/");

casper.start("https://www.instagram.com/"+ targetAccount +"/"
).waitForSelector("._devkn", function() {
  if (casper.exists("._kcrwx")) {
    console.log("Account is private");
  } else {
    console.log("Account is public");
  }
}).waitForSelector("div._mck9w a", function() {
  t1 = performance.now();
  returnHref = this.evaluate(enterPost);
  returnHref.length = 1;
  casper.click("a[href^='" + returnHref + "']");
  console.log(returnHref);
}).then(function() {
  profilePicture(dirtySrcSets, imgNames);
  checkAndGrab(dirtySrcSets, imgNames);
}).waitFor(function check(){
  t2 = performance.now();
  return casperDone;
}).then(function() {
  uniq(dirtySrcSets);
  console.log("Number of Links: " + finalisedLinks.length);
  uniqNames(imgNames);
  console.log("Number of Names: " + finalisedNames.length);
  t3 = performance.now();
  for (var i = 0; i<finalisedLinks.length; i++) {
    // console.log("Links: " + finalisedLinks[i] + " Name: " + finalisedNames[i]);
    if (String(finalisedLinks[i]).indexOf("mp4") > 0) {
      casper.download(finalisedLinks[i], "/home/ryan/Pictures/" + targetAccount + "/" + finalisedNames[i] + ".mp4");
    } else {
      casper.download(finalisedLinks[i], "/home/ryan/Pictures/" + targetAccount + "/" + finalisedNames[i] + ".jpeg");
    }

  }
  t4 = performance.now();
  console.log("Time to load page: " + (t1-t0));
  console.log("Time to grab all srcsets: " + (t2-t1));
  console.log("Time per post: " + (t2-t1)/finalisedNames.length);
  console.log("Time to clean arrays: " + (t3-t2));
  console.log("Time to download: " + (t4-t3));
  console.log("Total time taken: " + (t4-t0));
})

casper.run();

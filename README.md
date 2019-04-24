# InstagramScraper

## There are currently problems with image capture on stories, as well as intermittent issues with images on posts

This project exists to be the fastest and most reliable way to gather Instagram posts, stories, and comments. The alternatives that already existed when I started seemed to be quite poor, or simply no longer maintained. Since then, I've expanded this from a project written in JS and ran via the terminal into a fully fledged C# application with a GUI. My continued goal is to create the fastest, most lightweight tool for scraping Instagram that exists. 

As of now, future plans for the project include smart text download (so that it can handle deleted comments and the like), and better error handling. While I've done what I can to ensure the program will not crash, little information is given back to the user, which I'd like to change.

This has only been tested so far on a Linux system, so I have no idea whether or not it will function on Windows. I've added a few bits of functionality here and there to try to ensure it was cross platform, but seeing as how I won't be using this on Windows, there's no guarantee of full functionality.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for use.

Current options/abilities of the program:

- Gather all post images made by a user
- Gather all current stories made by a user
- Gather all comments made on a user's posts (including their bio)

### Prerequisites

What things you need:

```
.Net Core 2.2
```

### Running

Navigate to ~/Scraper and run the following command in the terminal:

```
dotnet run
```

A GUI will appear that you can use to run the program. You will need to provide a username to target (case insensative), and you can optionally put in your own username and password. In the case of story capture, Instagram does force a login, even on otherwise public profiles. None of your credentials are stored anywhere; the program simply transfers what you put into the text box to the Instagram login form.

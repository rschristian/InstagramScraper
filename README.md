# InstagramScraper

This project exists to be the fastest and most reliable way to gather Instagram posts, stories, and comments. The alternatives that already existed when I started seemed to be quite poor, or simply no longer maintained. From that point, the project started as a small JS program ran via the command line, but has since been converted to a C# Selenium project with a GTKSharp GUI. My continued main goal is to create the fastest, most lightweight tool for scraping Instagram that exists. 

As of now, future plans for the project include smart text download (so that it can handle deleted comments, as well as adding new ones), and better error handling. While I've tried to ensure the program won't out-right crash, there are a few troublesome CSS identifiers that can cause crashes if my assumptions are wrong.

This has only been tested so far on a Linux system, so usage anywhere else is completely new territory. I've added a few bits of functionality here and there to try to ensure it was cross platform, but there's no guarantee of functionality. Use at your own discretion, as it does have the power to rename and move files in some cases.

There are no future plans for this project, I have no plans to pick it back up if it does not still work.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for use.

Current options/abilities of the program:

- Download all post images made by a user
- Download all current stories made by a user
- Download all comments made on a user's posts (including their bio)

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

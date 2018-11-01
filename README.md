

This project exists as both a learning exercise and a method to go about data retrieval from Instagram. Whether it is for the backup of data, or use in another project (client's goal was to create a collage from all of a user's uploads), this will allow quick download of all images on a profile as well as optionally downloads all comments to text files, nicely dated and stored.

Dependencies - Casperjs, Slimerjs

A headless browser was needed to parse Instagram, as it utilizes React, and therfore a simple reading of the DOM was insufficient. Casper is used for it's naviagtion utilites, and slimer is the underlying engine used to collect most of the data. Slimer was chosen over PhantomJS as slimer still is being developed, runs on Gecko (important now that Chrome and Opera have moved on to Blink), and has a handful more tools that make certain operations much easier. 

There is a C# GUI that is made using WinForms, though it will need to be ported over to WPF.

Run (on Linux systems) with: csc instaScraper.cs -r:System.Windows.Forms.dll to compile, and mono instaScraper.exe to run.

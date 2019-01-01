using System;
using Gtk;
using System.IO;

namespace Selenium.UserInterface
{
	public class MainWindow : Window
	{
		private readonly Entry _targetAccount, _password, _username;
		private readonly CheckButton _headlessBrowserBox, _getStoryBox, _getTextBox;
		private readonly Button button;
		private readonly RadioButton _firefoxRadioButton, _chromeRadioButton;
		private readonly FileChooserWidget _fileChooserWidget;
//		private bool captureStory, captureText;
        
		public MainWindow() : base(WindowType.Toplevel)
		{
			// Widget MainWindow
			Name = "MainWindow";
			Title = "Instagram Scraper";
			SetIconFromFile(Directory.GetCurrentDirectory() + "/Resources/InstaScraper.ico");
			WindowPosition = WindowPosition.Center;
			BorderWidth = 3;
	        
			var alignment = new Alignment(0.5F, 0.5F, 1F, 1F) {Name = "alignment"};
			var fixedContainer = new Fixed {Name = "fixedContainer", HasWindow = false};
	        
			_targetAccount = new Entry
			{
				CanFocus = true, Name = "TargetAccount", Text = "gwenddalyn", IsEditable = true, PlaceholderText = "Target Account"
			};
			fixedContainer.Add(_targetAccount);
			var w1 = ((Fixed.FixedChild)(fixedContainer[_targetAccount]));
			w1.X = 10;
			w1.Y = 25;
			
			_username = new Entry
			{
				CanFocus = true, Name = "Username", IsEditable = true, PlaceholderText = "Account Username"
			};
			fixedContainer.Add(_username);
			var w2 = ((Fixed.FixedChild)(fixedContainer[_username]));
			w2.X = 10;
			w2.Y = 80;
			
			_password = new Entry
			{
				CanFocus = true, Name = "Password", IsEditable = true, PlaceholderText = "Account Password"
			};
			fixedContainer.Add(_password);
			var w3 = ((Fixed.FixedChild)(fixedContainer[_password]));
			w3.X = 10;
			w3.Y = 135;
	        
	        
			// Container child fixed1.Gtk.Fixed+FixedChild
			_headlessBrowserBox = new CheckButton
			{
				Active = true,
				CanFocus = true,
				Name = "HeadlessCheckBox",
				DrawIndicator = true,
				UseUnderline = true,
				Label = "Run in Headless Mode"
			};
			fixedContainer.Add(_headlessBrowserBox);
			var w4 = (Fixed.FixedChild)fixedContainer[_headlessBrowserBox];
			w4.X = 10;
			w4.Y = 190;
			
			_getStoryBox = new CheckButton
			{
				CanFocus = true,
				Name = "StoryCheckBox",
				DrawIndicator = true,
				UseUnderline = true,
				Label = "Download the Target Account's Story"
			};
			fixedContainer.Add(_getStoryBox);
			var w5 = (Fixed.FixedChild)fixedContainer[_getStoryBox];
			w5.X = 10;
			w5.Y = 225;
			
			_getTextBox = new CheckButton
			{
				CanFocus = true,
				Name = "TextCheckBox",
				DrawIndicator = true,
				UseUnderline = true,
				Label = "Download the Comments on the Target Account's Posts"
			};
			fixedContainer.Add(_getTextBox);
			var w6 = (Fixed.FixedChild)fixedContainer[_getTextBox];
			w6.X = 10;
			w6.Y = 260;
			
			_firefoxRadioButton = new RadioButton (null, "FireFoxRadioButton")
			{
				CanFocus = true,
				Name = "FireFoxRadioButton",
				DrawIndicator = true,
				UseUnderline = true,
				Label = "FireFox"
			};
			fixedContainer.Add(_firefoxRadioButton);
			var w7 = (Fixed.FixedChild)fixedContainer[_firefoxRadioButton];
			w7.X = 10;
			w7.Y = 295;
			
			_chromeRadioButton = new RadioButton (_firefoxRadioButton, "ChromeRadioButton")
			{
				CanFocus = true,
				Name = "ChromeRadioButton",
				DrawIndicator = true,
				UseUnderline = true,
				Label = "Chrome"
			};
			fixedContainer.Add(_chromeRadioButton);
			var w8 = (Fixed.FixedChild)fixedContainer[_chromeRadioButton];
			w8.X = 10;
			w8.Y = 330;

			button = new Button
			{
				CanFocus = true,
				Name = "RunScraperButton",
				UseUnderline = true,
				Label = "Run Web Scraper",
				HasFocus = true
			};
			button.Clicked += OnClickedEvent;
			
			fixedContainer.Add(button);
			var w9 = (Fixed.FixedChild)fixedContainer[button];
			w9.X = 10;
			w9.Y = 370;

			_fileChooserWidget = new FileChooserWidget(FileChooserAction.SelectFolder)
			{
				Name = "FileChooserWidget"
			};
			fixedContainer.Add(_fileChooserWidget);
			var w10 = (Fixed.FixedChild)fixedContainer[_fileChooserWidget];
			w10.X = 300;
			w10.Y = 300;

	        
			alignment.Add(fixedContainer);
			Add(alignment);
			Child?.ShowAll();
			DefaultWidth = 720;
			DefaultHeight = 480;
			Show();
			DeleteEvent += OnDeleteEvent;
		}

		private void OnClickedEvent(object obj, EventArgs args)
		{
			if (_targetAccount.Text != "")
			{
				WebScraper.SetUp(_targetAccount.Text, _fileChooserWidget.Uri.Substring(7), _headlessBrowserBox.Active, _firefoxRadioButton.Active);
			}
			else {var md = new MessageDialog(this, 
					DialogFlags.DestroyWithParent, MessageType.Error, 
					ButtonsType.Close, "You must give a target account");
				md.Run();
				md.Destroy();}
		}

		private static void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
			a.RetVal = true;
		}
	}
}   
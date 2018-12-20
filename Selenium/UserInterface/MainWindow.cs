using System;
using Gtk;

namespace Selenium.UserInterface
{
	public class MainWindow : Window
	{
		private readonly Entry targetAccount, password, username;
		private readonly CheckButton headlessBrowserBox;
		private readonly Button button;
		private bool captureStory, captureText;
        
		public MainWindow() : base(WindowType.Toplevel)
		{
			// Widget MainWindow
			Name = "MainWindow";
			Title = "Instagram Scraper";
			WindowPosition = (WindowPosition)4;
			BorderWidth = 3;
	        
	        
			//Container child MainWindow.Gtk.Container+ContainerChild
			var alignment = new Alignment(0.5F, 0.5F, 1F, 1F) {Name = "alignment"};
	        
	        
			// Container child alignment.Gtk.Container+ContainerChild
			var fixedContainer = new Fixed {Name = "fixedContainer", HasWindow = false};
	        
	        
			// Container child fixed1.Gtk.Fixed+FixedChild
			targetAccount = new Entry
			{
				CanFocus = true, Name = "TargetAccount", IsEditable = true, PlaceholderText = "Target Account"
			};
			fixedContainer.Add(targetAccount);
			var w1 = ((Fixed.FixedChild)(fixedContainer[targetAccount]));
			w1.X = 40;
			w1.Y = 20;
	        
	        
			// Container child fixed1.Gtk.Fixed+FixedChild
			headlessBrowserBox = new CheckButton
			{
				CanFocus = true,
				Name = "checkbutton2",
				DrawIndicator = true,
				UseUnderline = true,
				Label = "checkbutton2"
			};
			fixedContainer.Add(headlessBrowserBox);
			var w2 = (Fixed.FixedChild)fixedContainer[headlessBrowserBox];
			w2.X = 238;
			w2.Y = 217;

			button = new Button
			{
				CanFocus = true,
				Name = "run",
				UseUnderline = true,
				Label = "Run",
				HasFocus = true
			};
			button.Clicked += OnClickedEvent;
			
			fixedContainer.Add(button);
			var w3 = (Fixed.FixedChild)fixedContainer[button];
			w3.X = 238;
			w3.Y = 237;
	        
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
			WebScraper.SetUp(targetAccount.Text);
		}

		private static void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
			a.RetVal = true;
		}
	}
}   
using System;
using System.IO;
using System.Text;
using Gtk;
using Instagram_Scraper.Utility;

namespace Instagram_Scraper.UserInterface
{
    public class MainWindow : Window
    {
        private readonly RadioButton _firefoxRadioButton;
        private readonly CheckButton _headlessBrowserBox, _getStoryBox, _getCommentsBox, _onlyGetStoryBox;
        private readonly Entry _targetAccount, _password, _username, _savePath;

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
                CanFocus = true, Name = "TargetAccount", IsEditable = true, PlaceholderText = "Target Account"
            };
            fixedContainer.Add(_targetAccount);
            var w1 = (Fixed.FixedChild) fixedContainer[_targetAccount];
            w1.X = 10;
            w1.Y = 25;

            _username = new Entry
            {
                CanFocus = true, Name = "Username", IsEditable = true, PlaceholderText = "Account Username"
            };
            fixedContainer.Add(_username);
            var w2 = (Fixed.FixedChild) fixedContainer[_username];
            w2.X = 10;
            w2.Y = 80;

            _password = new Entry
            {
                CanFocus = true, Name = "Password", IsEditable = true, PlaceholderText = "Account Password"
            };
            fixedContainer.Add(_password);
            var w3 = (Fixed.FixedChild) fixedContainer[_password];
            w3.X = 10;
            w3.Y = 135;


            _headlessBrowserBox = new CheckButton
            {
                Active = true, CanFocus = true, Name = "HeadlessCheckBox", DrawIndicator = true, UseUnderline = true,
                Label = "Run in Headless Mode"
            };
            fixedContainer.Add(_headlessBrowserBox);
            var w4 = (Fixed.FixedChild) fixedContainer[_headlessBrowserBox];
            w4.X = 10;
            w4.Y = 190;

            _getStoryBox = new CheckButton
            {
                CanFocus = true, Name = "StoryCheckBox", DrawIndicator = true, UseUnderline = true,
                Label = "Download the Target Account's Story"
            };
            fixedContainer.Add(_getStoryBox);
            var w5 = (Fixed.FixedChild) fixedContainer[_getStoryBox];
            w5.X = 10;
            w5.Y = 225;

            _getCommentsBox = new CheckButton
            {
            	CanFocus = true, Name = "CommentCheckBox", DrawIndicator = true, UseUnderline = true,
            	Label = "Download the Comments on the Target Account's Posts"
            };
            fixedContainer.Add(_getCommentsBox);
            var w6 = (Fixed.FixedChild)fixedContainer[_getCommentsBox];
            w6.X = 10;
            w6.Y = 260;
            
            _onlyGetStoryBox = new CheckButton
            {
                CanFocus = true, Name = "OnlyStoryCheckBox", DrawIndicator = true, UseUnderline = true,
                Label = "Only get the User's story"
            };
            fixedContainer.Add(_onlyGetStoryBox);
            var w12 = (Fixed.FixedChild)fixedContainer[_onlyGetStoryBox];
            w12.X = 400;
            w12.Y = 190;


            _firefoxRadioButton = new RadioButton(null, "FireFoxRadioButton")
            {
                CanFocus = true, Name = "FireFoxRadioButton", DrawIndicator = true, UseUnderline = true,
                Label = "FireFox"
            };
            fixedContainer.Add(_firefoxRadioButton);
            var w7 = (Fixed.FixedChild) fixedContainer[_firefoxRadioButton];
            w7.X = 10;
            w7.Y = 295;

            var chromeRadioButton = new RadioButton(_firefoxRadioButton, "ChromeRadioButton")
            {
                CanFocus = true, Name = "ChromeRadioButton", DrawIndicator = true, UseUnderline = true,
                Label = "Chrome", Active = true
            };
            fixedContainer.Add(chromeRadioButton);
            var w8 = (Fixed.FixedChild) fixedContainer[chromeRadioButton];
            w8.X = 10;
            w8.Y = 330;
            

            var runProgramButton = new Button
            {
                CanFocus = true, Name = "RunScraperButton", UseUnderline = true, Label = "Run Web Scraper",
                HasFocus = true
            };
            runProgramButton.Clicked += OnClickedEvent;
            fixedContainer.Add(runProgramButton);
            var w9 = (Fixed.FixedChild) fixedContainer[runProgramButton];
            w9.X = 10;
            w9.Y = 370;

            _savePath = new Entry
            {
                CanFocus = true, Name = "SavePath", IsEditable = true,
                PlaceholderText = "Save Path (Default is Home/Pictures/[Account-Name])"
            };
            fixedContainer.Add(_savePath);
            var w10 = (Fixed.FixedChild) fixedContainer[_savePath];
            w10.X = 300;
            w10.Y = 300;

            var chooseSavePathButton = new Button
            {
                CanFocus = true, Name = "ChooseSavePathButton", UseUnderline = true, Label = "Select File Save Path",
                HasFocus = true
            };
            chooseSavePathButton.Clicked += OnClickedEvent;
            fixedContainer.Add(chooseSavePathButton);
            var w11 = (Fixed.FixedChild) fixedContainer[chooseSavePathButton];
            w11.X = 467;
            w11.Y = 301;


            alignment.Add(fixedContainer);
            Add(alignment);
            Child?.ShowAll();
            DefaultWidth = 720;
            DefaultHeight = 480;
            Show();
            DeleteEvent += OnDeleteEvent;
        }

        private void OnClickedEvent(object sender, EventArgs args)
        {
            if (!(sender is Button clickedButton)) return;

            if (clickedButton.Name.Equals("RunScraperButton"))
            {
                //Error Validation
                var errorMessages = new StringBuilder();

                if (_targetAccount.Text.Equals(string.Empty))
                    errorMessages.Append("You must provide a target account\n");

                if (_getStoryBox.Active && (_username.Text.Equals(string.Empty) ||
                                            _password.Text.Equals(string.Empty)))
                    errorMessages.Append("In order to view stories, Instagram requires login details");

                if (!errorMessages.ToString().Equals(string.Empty))
                {
                    var errorMessageDialog = new MessageDialog(this,
                        DialogFlags.DestroyWithParent, MessageType.Error,
                        ButtonsType.Close, errorMessages.ToString());
                    errorMessageDialog.Run();
                    errorMessageDialog.Destroy();
                }
                else
                {
                    WebScraper.SetUp(new ScraperOptions(_targetAccount.Text, _username.Text, _password.Text,
                        _headlessBrowserBox.Active, _getStoryBox.Active,  _getCommentsBox.Active,
                        _firefoxRadioButton.Active, _savePath.Text));
                }
            }
            else if (clickedButton.Name.Equals("ChooseSavePathButton"))
            {
                var fileChooserDialog = new FileChooserDialog("Choose a file", this,
                    FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept);

                if (fileChooserDialog.Run() == (int) ResponseType.Accept) _savePath.Text = fileChooserDialog.Filename;

                fileChooserDialog.Destroy();
            }
        }

        private static void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }
    }
}
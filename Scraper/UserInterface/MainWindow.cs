using System;
using System.IO;
using System.Text;
using Gtk;
using Instagram_Scraper.Domain;
using Instagram_Scraper.Utility;
using NLog;

namespace Instagram_Scraper.UserInterface
{
    public class MainWindow : Window
    {
        private static readonly Logger Logger = LogManager.GetLogger("Main Window");
        private readonly CheckButton _headlessBrowserBox, _getStoryBox, _getCommentsBox, _onlyGetStoryBox;
        private readonly Entry _targetAccount, _password, _username, _savePath;

        public MainWindow() : base(WindowType.Toplevel)
        {
            // Widget MainWindow
            Name = "MainWindow";
            Title = "InstaScraper";
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
            var targetAccountView = (Fixed.FixedChild) fixedContainer[_targetAccount];
            targetAccountView.X = 10;
            targetAccountView.Y = 25;

            _username = new Entry
            {
                CanFocus = true, Name = "Username", IsEditable = true, PlaceholderText = "Account Username"
            };
            fixedContainer.Add(_username);
            var usernameView = (Fixed.FixedChild) fixedContainer[_username];
            usernameView.X = 10;
            usernameView.Y = 80;

            _password = new Entry
            {
                CanFocus = true, Name = "Password", IsEditable = true, PlaceholderText = "Account Password",
                Visibility = false
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
            var headlessView = (Fixed.FixedChild) fixedContainer[_headlessBrowserBox];
            headlessView.X = 10;
            headlessView.Y = 190;

            _getStoryBox = new CheckButton
            {
                CanFocus = true, Name = "StoryCheckBox", DrawIndicator = true, UseUnderline = true,
                Label = "Download the Target Account's Story"
            };
            _getStoryBox.Clicked += OnClickedEvent;
            fixedContainer.Add(_getStoryBox);
            var storyView = (Fixed.FixedChild) fixedContainer[_getStoryBox];
            storyView.X = 10;
            storyView.Y = 225;

            _getCommentsBox = new CheckButton
            {
            	CanFocus = true, Name = "CommentCheckBox", DrawIndicator = true, UseUnderline = true,
            	Label = "Download the Comments on the Target Account's Posts"
            };
            fixedContainer.Add(_getCommentsBox);
            var getCommentsView = (Fixed.FixedChild)fixedContainer[_getCommentsBox];
            getCommentsView.X = 10;
            getCommentsView.Y = 260;
            
            _onlyGetStoryBox = new CheckButton
            {
                CanFocus = true, Name = "OnlyStoryCheckBox", DrawIndicator = true, UseUnderline = true,
                Label = "Only get the User's story", ChildVisible = false
            };
            fixedContainer.Add(_onlyGetStoryBox);
            var onlyStoryView = (Fixed.FixedChild)fixedContainer[_onlyGetStoryBox];
            onlyStoryView.X = 400;
            onlyStoryView.Y = 225;


            var runProgramButton = new Button
            {
                CanFocus = true, Name = "RunScraperButton", UseUnderline = true, Label = "Run Web Scraper",
                HasFocus = true
            };
            runProgramButton.Clicked += OnClickedEvent;
            fixedContainer.Add(runProgramButton);
            var runView = (Fixed.FixedChild) fixedContainer[runProgramButton];
            runView.X = 10;
            runView.Y = 300;

            _savePath = new Entry
            {
                CanFocus = true, Name = "SavePath", IsEditable = true,
                PlaceholderText = "Save Path (Default is Home/Pictures/[Account-Name])"
            };
            fixedContainer.Add(_savePath);
            var savePathEntryView = (Fixed.FixedChild) fixedContainer[_savePath];
            savePathEntryView.X = 300;
            savePathEntryView.Y = 300;

            var chooseSavePathButton = new Button
            {
                CanFocus = true, Name = "ChooseSavePathButton", UseUnderline = true, Label = "Select File Save Path",
                HasFocus = true
            };
            chooseSavePathButton.Clicked += OnClickedEvent;
            fixedContainer.Add(chooseSavePathButton);
            var savePathButtonView = (Fixed.FixedChild) fixedContainer[chooseSavePathButton];
            savePathButtonView.X = 467;
            savePathButtonView.Y = 301;


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

                if (!_onlyGetStoryBox.ChildVisible) _onlyGetStoryBox.Active = false;

                if (_targetAccount.Text.Equals(string.Empty))
                    errorMessages.Append("You must provide a target account\n");
                
                if (!_username.Text.Equals(string.Empty) &&
                    _password.Text.Equals(string.Empty) ||
                    _username.Text.Equals(string.Empty) &&
                    !_password.Text.Equals(string.Empty))
                    errorMessages.Append("Please fill out both the username and password fields, or leave them blank\n");

                if (_getStoryBox.Active && (_username.Text.Equals(string.Empty) ||
                                            _password.Text.Equals(string.Empty)))
                    errorMessages.Append("In order to view stories, Instagram requires login details\n");
                
                if (_onlyGetStoryBox.Active && !_getStoryBox.Active)
                    errorMessages.Append("Please select the \"Download the Target Account's Story\" to continue with the capture of only the user's story\n");
                
                if (_onlyGetStoryBox.Active && _getCommentsBox.Active)
                    errorMessages.Append("The \"Only Get Story\" option is not allowed to be used in conjunction with retrieving comments\n");

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
                    try
                    {
                        InitializeScraper.SetUp(new ScraperOptions(_targetAccount.Text, _username.Text, _password.Text,
                            _headlessBrowserBox.Active, _getStoryBox.Active,  _getCommentsBox.Active,
                            _onlyGetStoryBox.Active, _savePath.Text));
                    }
                    catch (Exception e)
                    {
                        Logger.Debug("Exception " + e + " was thrown.");
                    }
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
            else if (clickedButton.Name.Equals("StoryCheckBox"))
            {
                _onlyGetStoryBox.ChildVisible = !_onlyGetStoryBox.ChildVisible;
            }
        }

        private static void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace instagram_scraper
{
    public class ScraperGui : Form
    {
        private readonly CheckBox _headlessBrowserBox;
        private readonly TextBox _targetAccount, _password, _username;
        private bool _captureStory, _captureText;

        public ScraperGui()
        {
            Text = "Instagram Scraper";
            Size = new Size(400, 400);

            var mainMenu = new MainMenu();
            var account = mainMenu.MenuItems.Add("&Account");
            account.MenuItems.Add(new MenuItem("&Exit", OnExit, Shortcut.CtrlX));

            Menu = mainMenu;

            var groupBox = new GroupBox();

            _targetAccount = new TextBox {Location = new Point(0, 10), Text = "***REMOVED***", Size = new Size(120, 20)};

            _username = new TextBox {Location = new Point(0, 50), Text = "***REMOVED***", Size = new Size(120, 20)};

            _password = new TextBox {Location = new Point(0, 100), Text = "***REMOVED***", Size = new Size(120, 20)};

            groupBox.Controls.Add(_targetAccount);
            groupBox.Controls.Add(_username);
            groupBox.Controls.Add(_password);
            groupBox.AutoSize = true;
            groupBox.Enabled = true;
            Controls.Add(groupBox);

            var storyCaptureBox = new CheckBox
            {
                Parent = this, Location = new Point(30, 150), Text = "Capture Story", Checked = false
            };
            storyCaptureBox.CheckedChanged += delegate { OnChanged(0); };

            _headlessBrowserBox = new CheckBox
            {
                Parent = this, Location = new Point(30, 180), Text = "Headless", Checked = true
            };

            var textCaptureBox = new CheckBox
            {
                Parent = this, Location = new Point(30, 210), Text = "Download Text", Checked = false
            };
            textCaptureBox.CheckedChanged += delegate { OnChanged(1); };

            var btn1 = new Button {Text = "Run", Parent = this, Location = new Point(30, 250)};
            btn1.Click += button_Click;

            Controls.Add(btn1);

            try
            {
                Icon = new Icon("resources/InstaScraper.ico");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            CenterToScreen();
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void OnChanged(int checkbox)
        {
            switch (checkbox)
            {
                case 0:
                    _captureStory = !_captureStory;
                    break;
                case 1:
                    _captureText = !_captureText;
                    break;
                default:
                    Console.WriteLine("Bad onChanged Event");
                    break;
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, EventArgs e)
        {
            var publicArgs = String.Format("casperjs --engine=slimerjs resources/js/PublicProfiles.js --targetAccount" +
                                           "='" + _targetAccount.Text + "' --retrieveText='" + _captureText +
                                           "' --captureStory='" + _captureStory + "'");

            var privateArgs = String.Format("casperjs --engine=slimerjs resources/js/LoginProfiles.js --targetAccount" +
                                            "='" + _targetAccount.Text + "' --retrieveText='" + _captureText +
                                            "' --captureStory='" + _captureStory + "' --username='" +
                                            _username.Text + "' --password='" + _password.Text + "' -P newProfile");

            if (_headlessBrowserBox.Checked)
            {
                publicArgs = publicArgs + " --headless";
                privateArgs = privateArgs + " --headless";
            }

            if (_targetAccount.Text == "Target Account")
            {
                MessageBox.Show("Must provide a valid account", "Instagram Scraper",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (_captureStory && _username.Text == "Account Username")
            {
                MessageBox.Show("Capturing stories requires a valid login, due to Instagram's " +
                                "policies. Please either deselect 'Capture Story', or input login details",
                    "Instagram Scraper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (_username.Text != "Account Username" && _password.Text != "Account Password")
            {
                ExecuteScript(privateArgs);
            }
            else if (_username.Text == "Account Username")
            {
                ExecuteScript(publicArgs);
            }
        }

        private static void ExecuteScript(string casperArguments)
        {
            var p = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \" " + casperArguments + " \"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            p.Start();

            while (!p.StandardOutput.EndOfStream)
            {
                Console.WriteLine(p.StandardOutput.ReadLine());
            }
        }

        void OnPrivateReturn(object sender, EventArgs e)
        {
            MessageBox.Show("Sorry, but the profile is private. You will need to log-in" +
                            " to download this user's posts.", "Instagram Scraper", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
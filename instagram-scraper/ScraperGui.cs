using System;
using System.Drawing;
using System.Windows.Forms;

namespace instagram_scraper
{
    public class ScraperGui : Form
    {
        private CheckBox storyCaptureBox, headlessBrowserBox, textCaptureBox;
        private TextBox targetAccount, password, username;
        private Button btn1;
        private bool captureStory, captureText;

        public ScraperGui()
        {
            Text = "Instagram Scraper";
            Size = new Size(400, 400);

            MainMenu mainMenu = new MainMenu();
            MenuItem account = mainMenu.MenuItems.Add("&Account");
            account.MenuItems.Add(new MenuItem("&Exit",
                new EventHandler(this.OnExit), Shortcut.CtrlX));

            Menu = mainMenu;

            StatusBar sb = new StatusBar();
            sb.Parent = this;
            sb.Text = "Ready";


            GroupBox groupbox = new GroupBox();

            targetAccount = new TextBox();
            targetAccount.Location = new Point(0, 10);
            targetAccount.Text = "elisegahart";
            targetAccount.Size = new Size(120, 20);

            username = new TextBox();
            username.Location = new Point(0, 50);
            username.Text = "thunfremlinc";
            username.Size = new Size(120, 20);

            password = new TextBox();
            password.Location = new Point(0, 100);
            password.Text = "Ma princesse";
            password.Size = new Size(120, 20);

            groupbox.Controls.Add(targetAccount);
            groupbox.Controls.Add(username);
            groupbox.Controls.Add(password);
            groupbox.AutoSize = true;
            groupbox.Enabled = true;
            Controls.Add(groupbox);

            storyCaptureBox = new CheckBox();
            storyCaptureBox.Parent = this;
            storyCaptureBox.Location = new Point(30, 150);
            storyCaptureBox.Text = "Capture Story";
            storyCaptureBox.Checked = false;
            storyCaptureBox.CheckedChanged += delegate(object sender, EventArgs e) { OnChanged(sender, e, 0); };

            headlessBrowserBox = new CheckBox();
            headlessBrowserBox.Parent = this;
            headlessBrowserBox.Location = new Point(30, 180);
            headlessBrowserBox.Text = "Headless";
            headlessBrowserBox.Checked = true;

            textCaptureBox = new CheckBox();
            textCaptureBox.Parent = this;
            textCaptureBox.Location = new Point(30, 210);
            textCaptureBox.Text = "Download Text";
            textCaptureBox.Checked = false;
            textCaptureBox.CheckedChanged += delegate(object sender, EventArgs e) { OnChanged(sender, e, 1); };

            btn1 = new Button();
            btn1.Text = "Run";
            btn1.Parent = this;
            btn1.Location = new Point(30, 250);
            btn1.Click += new EventHandler(button_Click);

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

        private void OnChanged(object sender, EventArgs e, int checkbox)
        {
            switch (checkbox)
            {
                case 0:
                    captureStory = !captureStory;
                    break;
                case 1:
                    captureText = !captureText;
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
                                           "='" + targetAccount.Text + "' --retrieveText='" + captureText +
                                           "' --captureStory='" + captureStory + "'");

            var privateArgs = String.Format("casperjs --engine=slimerjs resources/js/LoginProfiles.js --targetAccount" +
                                            "='" + targetAccount.Text + "' --retrieveText='" + captureText +
                                            "' --captureStory='" + captureStory + "' --username='" +
                                            username.Text + "' --password='" + password.Text + "' -P newProfile");

            if (headlessBrowserBox.Checked)
            {
                publicArgs = (publicArgs + " --headless");
                privateArgs = (privateArgs + " --headless");
            }

            if (targetAccount.Text == "Target Account")
            {
                MessageBox.Show("Must provide a valid account", "Instagram Scraper",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (captureStory == true && username.Text == "Account Username")
            {
                MessageBox.Show("Capturing stories requires a valid login, due to Instagram's " +
                                "policies. Please either deselect 'Capture Story', or input login details",
                    "Instagram Scraper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (username.Text != "Account Username" && password.Text != "Account Password")
            {
                ExecuteScript(privateArgs);
            }
            else if (username.Text == "Account Username")
            {
                ExecuteScript(publicArgs);
            }
        }

        private void ExecuteScript(string casperArguments)
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = "-c \" " + casperArguments + " \"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
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
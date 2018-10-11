using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

public class MForm : Form
{
    private CheckBox storyCaptureBox, headlessBrowserBox;
    private TextBox targetAccount, password, username;
    private Button btn1;
    private bool captureStory = true, headless = true;
    
    public MForm()
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
        targetAccount.Text = "gwenddalyn";
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
        this.Controls.Add(groupbox);

        storyCaptureBox = new CheckBox();
        storyCaptureBox.Parent = this;
        storyCaptureBox.Location = new Point(30, 150);
        storyCaptureBox.Text = "Capture Story";
        storyCaptureBox.Checked = true;
        storyCaptureBox.CheckedChanged += delegate(object sender, EventArgs e) { OnChanged(sender, e, 0); };
        
        headlessBrowserBox = new CheckBox();
        headlessBrowserBox.Parent = this;
        headlessBrowserBox.Location = new Point(30, 170);
        headlessBrowserBox.Text = "Headless";
        headlessBrowserBox.Checked = true;
        headlessBrowserBox.CheckedChanged += delegate(object sender, EventArgs e) { OnChanged(sender, e, 1); };
        
        btn1 = new Button();
        btn1.Text = "Run";
        btn1.Parent = this;
        btn1.Location = new Point(30, 210);
        btn1.Click += new EventHandler(button_Click);

        Controls.Add(btn1);

        try
        {
            Icon = new Icon("InstaScraper.ico");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }

        CenterToScreen();
    }

    void OnChanged(object sender, EventArgs e, int checkbox)
    {
        switch (checkbox)
        {
            case 0:
                captureStory = !captureStory;
                Console.WriteLine(captureStory);
                break;
            case 1:
                headless = !headless;
                break;
        }    
    }

    void OnExit(object sender, EventArgs e)
    {
        Close();
    }
    
    private void button_Click(object sender, EventArgs e)
    {
        string arg = String.Format("casperjs --engine=slimerjs Master.js --targetAccount" +
                                   "='" + targetAccount.Text + "' --retrieveText='false'");
        if (headlessBrowserBox.Checked)
        {
            arg = (arg + " --headless");
        }

        if (captureStory == true && username.Text != "Account Username" && password.Text != "Account Password")
        {
            arg = (arg + " --captureStory='true' --username='" + username.Text + "' --password='"
                   + password.Text + "'");
        }

        ExecuteScript(arg);

    }

    private static void ExecuteScript(string casperArguments)
    {
        Process p = new System.Diagnostics.Process ();
        p.StartInfo.FileName = "/bin/bash";
        p.StartInfo.Arguments = "-c \" " + casperArguments + " \"";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        
        while (!p.StandardOutput.EndOfStream) {
            Console.WriteLine (p.StandardOutput.ReadLine ());
        }
    }
    
}

class MApplication {
    
    public static void Main()
    {
       Application.Run(new MForm());
    }
}

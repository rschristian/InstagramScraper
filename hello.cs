using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

public class MForm : Form
{
    private CheckBox story;
    private CheckBox retrieveText;
    private TextBox targetAccount;
    private Button btn1;
    
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

        story = new CheckBox();
        story.Parent = this;
        story.Location = new Point(30, 30);
        story.Text = "Capture Story";
        story.Checked = true;
        story.CheckedChanged += new EventHandler(OnChanged);
        
        retrieveText = new CheckBox();
        retrieveText.Parent = this;
        retrieveText.Location = new Point(30, 70);
        retrieveText.Text = "Capture Story";
        retrieveText.Checked = true;
        retrieveText.CheckedChanged += new EventHandler(OnChanged);

        targetAccount = new TextBox();
        targetAccount.Parent = this;
        targetAccount.Location = new Point(30, 110);
        targetAccount.Text = "Target Account";
        targetAccount.AcceptsReturn = true;
        targetAccount.AcceptsTab = true;
        targetAccount.Dock = DockStyle.Fill;

        btn1 = new Button();
        btn1.Text = "Run";
        btn1.Parent = this;
        btn1.Location = new Point(30, 110);
        btn1.Click += new EventHandler(button_Click);

        Controls.Add(btn1);

        try
        {
            Icon = new Icon("webd.ico");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }

        CenterToScreen();
    }

    void OnChanged(object sender, EventArgs e)
    {
        if (story.Checked)
        {
            Text = "Story is set to: true";
        }
        else if (retrieveText.Checked)
        {
            Text = "retrieve Text";
        }
        else
        {
            Text = "Instagram Scraper";
        }
    }

    void OnExit(object sender, EventArgs e)
    {
        Close();
    }
    
    private void button_Click(object sender, EventArgs e)
    {
        FileInfo csp1 = new FileInfo(@"/home/ryan/Repositories/casperjs/bin/casperjs");
        FileInfo csp2 = new FileInfo(@"/home/ryan/Repositories/casperjs/package.json");
        FileInfo slm = new FileInfo(@"/home/ryan/node_modules/slimerjs/src");
        string EnvPath = string.Format(";{0};{1}", slm, csp2);

        DirectoryInfo dir = csp1.Directory;
        FileInfo path = new FileInfo(@"/usr/bin/python3.6");
        Console.WriteLine(Environment.GetEnvironmentVariable("SLIMERJS"));

        string arg = String.Format("casperjs --engine=slimerjs Master.js --targetAccount" +
                                   "='***REMOVED***' --retrieveText='false' --username='***REMOVED***'" +
                                   " --password='***REMOVED***' --headless");
        ExecutePythonScript(dir, path, arg, EnvPath);

    }

    private static void ExecutePythonScript(DirectoryInfo workingDir, FileInfo pythonPath, string casperArguments, string EnvPath)
    {
        var p = new Process();
        p.StartInfo.EnvironmentVariables["PATH"] = EnvPath;
        p.StartInfo.WorkingDirectory = workingDir.FullName;
        p.StartInfo.FileName = pythonPath.FullName;
        p.StartInfo.Arguments = casperArguments;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;

        Console.WriteLine(EnvPath);

        p.ErrorDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                MessageBox.Show("e> " + e.Data);
        };

        p.OutputDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                MessageBox.Show("->" + e.Data);
        };

        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        p.WaitForExit();
        p.Close();
    }
    
}

class MApplication {
    
    public static void Main()
    {
       Application.Run(new MForm());
    }
}

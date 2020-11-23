using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchPlaysTrackmania
{
    public partial class Form1 : Form
    {
        public Stopwatch RespawnTimer = new Stopwatch();
        internal Settings Settings;
        internal Bot Bot;

        public void CheckAHKScripts()
        {
            string[] ahkScript = new string[]
            {
                "#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.",
                "; #Warn  ; Enable warnings to assist with detecting common errors.",
                "SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.",
                "SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.",
                "Send { Left down }",
                "Sleep %1%",
                "Send { Left }"
            };
            if (!File.Exists("left.ahk"))
            {
                File.WriteAllLines("left.ahk", ahkScript);
            }
            if (!File.Exists("right.ahk"))
            {
                ahkScript[4] = ahkScript[4].Replace("Left", "Right");
                ahkScript[6] = ahkScript[6].Replace("Left", "Right");
                File.WriteAllLines("right.ahk", ahkScript);
            }
            if (!File.Exists("up.ahk"))
            {
                ahkScript[4] = ahkScript[4].Replace("Right", "Up");
                ahkScript[6] = ahkScript[6].Replace("Right", "Up");
                File.WriteAllLines("up.ahk", ahkScript);
            }
            if (!File.Exists("down.ahk"))
            {
                ahkScript[4] = ahkScript[4].Replace("Up", "Down");
                ahkScript[6] = ahkScript[6].Replace("Up", "Down");
                File.WriteAllLines("down.ahk", ahkScript);
            }
        }
        public Form1()
        {
            RespawnTimer.Start();
            CheckAHKScripts();
            if (!File.Exists("Settings.json"))
            {
                Settings = new Settings();
                File.WriteAllText("Settings.json", JsonConvert.SerializeObject(Settings));
            }
            Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json"));

            InitializeComponent();

            Bot = new Bot();
            Bot.SetForm(this);

            ExecuteAction(() =>
            {
                UpCommandTextBox.Text = Settings.UpChatCommand;
                UpCommandDelayTextBox.Text = Settings.UpPressTime.ToString();

                DownCommandTextBox.Text = Settings.DownChatCommand;
                DownCommandDelayTextBox.Text = Settings.DownPressTime.ToString();

                LeftCommandTextBox.Text = Settings.LeftChatCommand;
                LeftCommandDelayTextBox.Text = Settings.LeftPressTime.ToString();

                RightCommandTextBox.Text = Settings.RightChatCommand;
                RightCommandDelayTextBox.Text = Settings.RightPressTime.ToString();
            });

            if (Settings.ChannelName != null && Settings.OAuth != null && Settings.Username != null)
            {
                ExecuteAction(() =>
                {
                    ChannelNameTextBox.Text = Settings.ChannelName;
                    OAuthTextBox.Text = Settings.OAuth;
                    UsernameTextBox.Text = Settings.Username;
                });
            }
            else
            {
                ExecuteAction(() =>
                {
                    DebugRichTextBox.AppendText("Please configure ChatBot settings\n");
                });
            }
        }

        public void ExecuteAction(Action makeAction)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(makeAction));
            else
                makeAction();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExecuteAction(() =>
            {
                DebugRichTextBox.AppendText("Settings saved\n");
            });

            Settings.ChannelName = ChannelNameTextBox.Text;
            Settings.OAuth = OAuthTextBox.Text;
            Settings.Username = UsernameTextBox.Text;

            File.WriteAllText("Settings.json", JsonConvert.SerializeObject(Settings));

            Task.Run(() =>
            {
                Bot.Connect(true, Settings);
            });
        }

        private void ShowUsernameButton_MouseDown(object sender, MouseEventArgs e)
        {
            UsernameTextBox.PasswordChar = '\0';
        }

        private void ShowUsernameButton_MouseUp(object sender, MouseEventArgs e)
        {
            UsernameTextBox.PasswordChar = '*';
        }

        private void ShowOAuthButton_MouseDown(object sender, MouseEventArgs e)
        {
            OAuthTextBox.PasswordChar = '\0';
        }

        private void ShowOAuthButton_MouseUp(object sender, MouseEventArgs e)
        {
            OAuthTextBox.PasswordChar = '*';
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                Settings.UpChatCommand = UpCommandTextBox.Text;
                Settings.UpPressTime = int.Parse(UpCommandDelayTextBox.Text);

                Settings.DownChatCommand = DownCommandTextBox.Text;
                Settings.DownPressTime = int.Parse(DownCommandDelayTextBox.Text);

                Settings.LeftChatCommand = LeftCommandTextBox.Text;
                Settings.LeftPressTime = int.Parse(LeftCommandDelayTextBox.Text);

                Settings.RightChatCommand = RightCommandTextBox.Text;
                Settings.RightPressTime = int.Parse(RightCommandDelayTextBox.Text);

                File.WriteAllText("Settings.json", JsonConvert.SerializeObject(Settings));
                ExecuteAction(() =>
                {
                    DebugRichTextBox.AppendText($"Key settings saved\n");
                });
            }
            catch (Exception ex)
            {
                ExecuteAction(() =>
                {
                    DebugRichTextBox.AppendText($"{ex.ToString()}\n");
                });
            }
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            ExecuteAction(() =>
            {
                if (Bot.Active)
                {
                    ToggleButton.Text = "Turn ON";
                    Bot.Active = false;
                    DebugRichTextBox.AppendText($"Disabled\n");
                }
                else
                {
                    ToggleButton.Text = "Turn OFF";
                    Bot.Active = true;
                    DebugRichTextBox.AppendText($"Enabled\n");
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            string[] dir =
            {
                "!up",
                "!left",
                "!right",
                "!down"
            };
            Task.Run(() =>
            {
                Thread.Sleep(2000);

                for (int i = 0; i < 500; i++)
                {
                    int randomSleep = r.Next(10, 300);
                    string randomDir = dir[r.Next(0, 3)];

                    if (randomDir == Settings.RightChatCommand)
                    {
                        Process.Start($"right.ahk", randomSleep.ToString());
                    }
                    else if (randomDir == Settings.LeftChatCommand)
                    {
                        Process.Start($"left.ahk", randomSleep.ToString());
                    }
                    else if (randomDir == Settings.UpChatCommand)
                    {
                        Process.Start($"up.ahk", randomSleep.ToString());
                    }
                    else if (randomDir == Settings.DownChatCommand)
                    {
                        Process.Start($"down.ahk", randomSleep.ToString());
                    }
                }
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bot.ResetCount();
        }
    }
}

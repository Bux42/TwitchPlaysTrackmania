using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchPlaysTrackmania
{
    class Bot
    {
        public string ChannelName = null;
        public ConnectionCredentials creds = null;
        public TwitchClient client;
        private Form1 form;

        private int RightCount = 0;
        private int LeftCount = 0;
        private int UpCount = 0;
        private int DownCount = 0;

        public bool Active = false;

        NumberFormatInfo Nfi;

        public void SetForm(Form1 form)
        {
            this.form = form;
            Nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            Nfi.NumberGroupSeparator = " ";
            Nfi.NumberDecimalDigits = 0;
        }
        internal void Connect(bool isLogging, Settings settings)
        {
            try
            {
                ChannelName = settings.ChannelName;
                creds = new ConnectionCredentials(settings.Username, settings.OAuth);

                client = new TwitchClient();
                client.Initialize(creds, ChannelName);

                form.ExecuteAction(() =>
                {
                    form.DebugRichTextBox.AppendText("[Bot]: Connecting...\n");
                });

                if (isLogging)
                    client.OnLog += Client_OnLog;

                client.OnError += Client_OnError;
                client.OnConnectionError += Client_OnConnectedError;
                client.OnMessageReceived += Client_OnMessageReceived;
                client.OnChatCommandReceived += Client_OnChatCommandReceived;
                client.OnFailureToReceiveJoinConfirmation += Client_OnFailureToReceiveJoinConfirmation;

                client.Connect();
                client.OnConnected += Client_OnConnected;
            }
            catch (Exception e)
            {
                form.ExecuteAction(() =>
                {
                    form.DebugRichTextBox.AppendText($"{e.ToString()}\n");
                });
            }
        }

        private void Client_OnFailureToReceiveJoinConfirmation(object sender, OnFailureToReceiveJoinConfirmationArgs e)
        {
            form.ExecuteAction(() =>
            {
                form.DebugRichTextBox.AppendText($"Authentification failed, are the settings okay?\n");
            });
        }

        private void Client_OnConnectedError(object sender, OnConnectionErrorArgs e)
        {
            form.ExecuteAction(() =>
            {
                form.DebugRichTextBox.AppendText($"{e.Error.Message}\n");
            });
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            /*
            form.ExecuteAction(() =>
            {
                form.DebugRichTextBox.AppendText($"[Command] {e.Command.CommandText}\n");
                client.SendMessage(ChannelName, "Ain't got no dice");
            });
            */
        }

        public void ResetCount()
        {
            RightCount = 0;
            LeftCount = 0;
            UpCount = 0;
            DownCount = 0;

            form.ExecuteAction(() =>
            {
                form.RightCountLabel.Text = RightCount.ToString("n", Nfi);
                form.LeftCountLabel.Text = LeftCount.ToString("n", Nfi);
                form.UpCountLabel.Text = UpCount.ToString("n", Nfi);
                form.DownCountLabel.Text = DownCount.ToString("n", Nfi);
            });
        }
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (Active)
            {
                form.ExecuteAction(() =>
                {
                    string command = e.ChatMessage.Message.ToLower();

                    if (command == form.Settings.RightChatCommand)
                    {
                        Process.Start($"right.ahk", form.Settings.RightPressTime.ToString());
                        RightCount++;
                        form.RightCountLabel.Text = RightCount.ToString("n", Nfi);
                    }
                    else if (command == form.Settings.LeftChatCommand)
                    {
                        Process.Start($"left.ahk", form.Settings.LeftPressTime.ToString());
                        LeftCount++;
                        form.LeftCountLabel.Text = LeftCount.ToString("n", Nfi);
                    }
                    else if (command == form.Settings.UpChatCommand)
                    {
                        Process.Start($"up.ahk", form.Settings.UpPressTime.ToString());
                        UpCount++;
                        form.UpCountLabel.Text = UpCount.ToString("n", Nfi);
                    }
                    else if (command == form.Settings.DownChatCommand)
                    {
                        Process.Start($"down.ahk", form.Settings.DownPressTime.ToString());
                        DownCount++;
                        form.DownCountLabel.Text = DownCount.ToString("n", Nfi);
                    }
                });
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            form.ExecuteAction(() =>
            {
                form.DebugRichTextBox.AppendText($"{e.Exception.ToString()}\n");
            });
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            form.ExecuteAction(() =>
            {
                form.DebugRichTextBox.AppendText("[Bot]: Connected\n");
                form.ConnectChatBotButton.Enabled = false;
                form.ToggleButton.Enabled = true;
            });
        }

        internal void Disconnect()
        {
            form.ExecuteAction(() =>
            {
                form.DebugRichTextBox.AppendText("[Bot]: Disonnecting and closing application\n");
            });

            client.Disconnect();
        }
    }
}

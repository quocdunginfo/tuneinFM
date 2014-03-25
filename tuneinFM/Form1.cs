using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreAudioApi;
using System.Threading;
using SpeechLib;
using System.Diagnostics;
namespace tuneinFM
{
    public partial class Form1 : Form
    {
        public delegate void SetTextCallback(string text);
        String[] channels = {
        "http://tunein.com/radio/FOX-News-Talk-s20431/",//FOX News Talk
        "http://tunein.com/radio/BBC-Radio-4-935-s25419/",//BBC 4
        "http://tunein.com/radio/Science360-Radio-s144232/",//Science 360
        "http://tunein.com/radio/PBS-Newshour-p47926/",//PBS News
        "http://tunein.com/radio/LBC-News-1152-s18616/",//LBC News London
        };
        String[] channels_name = {
        "FOX News Talk",
        "BBC 4",
        "Science 360",
        "PBS News",
        "LBC News London"
        };
        int index = 0;
        Thread demoThread;
        public Form1()
        {
            InitializeComponent();

            demoThread =
                new Thread(new ThreadStart(this.ThreadProcSafe));
            demoThread.Start();
        }

        public void ThreadProcSafe()
        {
            MuteCheck p = new MuteCheck();
            p.run();
            while(true)
            { 
                while(!p.mute_state)
                {
                    //chờ bấm mute
                }
                //close all chrome first
                CloseAllChromeBrowsers();
                Thread.Sleep(1000);//để kịp lên URL
                //swap channel
                index++;
                if (index >= channels.Length) index = 0;
                    //webBrowser1.Navigate(channels[index]);
                    System.Diagnostics.Process.Start(channels[index]);

                    try
                    {
                        this.textBox1.Focus();
                    }catch(Exception ex)
                    {
                        
                    }
                //
                Thread.Sleep(1000);//để kịp trả về unmute để nghe tên kênh
                //speake the channel name
                SpVoice voice = new SpVoice();
                voice.Speak(channels_name[index]);
                
                //reverse state
                p.mute_state = false;//thay đổi trạng thái lại ban đầu
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            demoThread.Abort();
            demoThread = null;
        }
        static void CloseAllChromeBrowsers()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("chrome"))
                {
                    //proc.CloseMainWindow();
                    //proc.Close();
                    proc.Kill();
                }
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("No instances of Notepad running.");
            }
        }
    }
    class MuteCheck
    {
        public bool mute_state=false;
        private MMDevice device;
        public void run()
        {
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            mute_state = data.Muted;
        }
    }
}

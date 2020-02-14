using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortCutSpotify
{
    public partial class TestDllForm : Form
    {
        SpotifyService.AnimationProvider AnimationProvider { get; set; }
        SpotifyService.SpotifyProvider SpotifyProvider { get; set; }

        GlobalHook GlobalHook { get; set; }

        private Boolean ControlButtonPressed { get; set; }

        //SpotifyService.User32 User32 { get; set; }
        public TestDllForm()
        {
            InitializeComponent();
            AnimationProvider = new SpotifyService.AnimationProvider();
            SpotifyProvider = new SpotifyService.SpotifyProvider();
            this.textBoxCurrentSongName.Text = SpotifyProvider.Connect();

            // Disable Windows OS Animation (minimize, maximize and so on..)
            AnimationProvider.DiableAnimation();

            // Set Window Opacity to 100%
            SpotifyProvider.ResetAndInitOpacity();

            // SetWIndow Borderless
            SpotifyProvider.MakeExternalWindowBorderless();

            GlobalHook = new GlobalHook();

            GlobalHook.HookedKeys.Add(Keys.LShiftKey);
            GlobalHook.HookedKeys.Add(Keys.Up);
            GlobalHook.HookedKeys.Add(Keys.Down);
            GlobalHook.HookedKeys.Add(Keys.Left);
            GlobalHook.HookedKeys.Add(Keys.Right);
            GlobalHook.HookedKeys.Add(Keys.Space);

            GlobalHook.KeyDown += new KeyEventHandler(hook_KeyDown); //Taste wird gedrückt
            GlobalHook.KeyUp += new KeyEventHandler(hook_KeyUp); //Taste wird losgelassen
        }

        async void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (ControlButtonPressed)
            {
                ControlButtonPressed = false;
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        ControlButtonPressed = false;
                        Console.WriteLine("PauseStopp Performed");
                        this.textBoxCurrentSongName.Text = await SpotifyProvider.PerformPlayAsync();
                        e.Handled = true;
                        return;
                    case Keys.End:
                        e.Handled = true;
                        return;
                    case Keys.Up:
                        SpotifyProvider.PerformIncreaseVolume();
                        e.Handled = true;
                        return;
                    case Keys.Down:
                        SpotifyProvider.PerformDecreaseVolume();
                        e.Handled = true;
                        return;
                    case Keys.Right:
                        this.textBoxCurrentSongName.Text = await SpotifyProvider.PerformNextSongAsync();
                        e.Handled = true;
                        return;
                    case Keys.Left:
                        this.textBoxCurrentSongName.Text = await SpotifyProvider.PerformPreviousSongAsync();
                        e.Handled = true;
                        return;
                    case Keys.LControlKey:
                        e.Handled = true;
                        return;

                    case Keys.LShiftKey:
                        e.Handled = true;
                        return;

                    default:
                        e.Handled = true;
                        return;
                }
            }

            if(Keys.LShiftKey == e.KeyCode)
            {
                ControlButtonPressed = true;
                e.Handled = true;
            }
        }

        void hook_KeyUp(object sender, KeyEventArgs e)
        {
            ControlButtonPressed = false;

            //Dein Code
            e.Handled = true;
        }


        /// <summary>
        /// NEXT SONG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button2_Click(object sender, EventArgs e)
        {
            this.textBoxCurrentSongName.Text = await SpotifyProvider.PerformNextSongAsync();
        }

        /// <summary>
        /// LAST SONG 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button3_Click(object sender, EventArgs e)
        {
            this.textBoxCurrentSongName.Text = await SpotifyProvider.PerformPreviousSongAsync();
        }

        /// <summary>
        /// PLAY PAUSE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button4_Click(object sender, EventArgs e)
        {
            this.textBoxCurrentSongName.Text = await SpotifyProvider.PerformPlayAsync();
        }

        /// <summary>
        /// INCEASE VOLUMNE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDecreaseVolume_Click(object sender, EventArgs e)
        {
            SpotifyProvider.PerformDecreaseVolume();
        }

        /// <summary>
        /// DECREASE VOLUMNE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonIncreaseVolume_Click(object sender, EventArgs e)
        {
            SpotifyProvider.PerformIncreaseVolume();
        }

        /// <summary>
        /// RESET WINDOW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            AnimationProvider.EnableAnimation();

            // Set Window Opacity to 100%
            SpotifyProvider.ResetAndInitOpacity();
            SpotifyProvider.ResetWindow();
        }
    }
}

using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] files;
        List<string> filesList_for_media = new List<string>();
        List<string> filesList_for_listbox = new List<string>();
        int selected_index = 0;
        // flag for choosing again
        bool play = true;
        bool play_again= false;
        bool mixing= false;
        Random random = new Random();
        List<string> copy = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Papka_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker=true};
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
               files = Directory.GetFiles(dialog.FileName);
            }
            foreach(string file in files)
            {
                if (file.EndsWith(".mp3") || file.EndsWith(".m4a") || file.EndsWith(".flac"))
                {
                    filesList_for_listbox.Add(file.Substring(file.LastIndexOf("\\") + 1, file.LastIndexOf('.') - file.LastIndexOf("\\")));
                    filesList_for_media.Add(file);
                    copy.Add(file);
                }
            }
            if(filesList_for_listbox.Count > 0 & filesList_for_media.Count > 0)
            {
                selected_index = 0;

                List.ItemsSource = filesList_for_listbox;
                Volume_slide.Value = 70;
                Volume_slide.Maximum = 100;
                media.Source = new Uri(filesList_for_media[selected_index]);
                List.SelectedIndex = selected_index;
                media.Volume = Volume_slide.Value / 100;
                media.Play();
                Thread thread = new Thread(_ =>
                {
                    potok();
                });
                thread.Start();
            }
        }

        private void Length_slide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(Length_slide.Value));
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (play_again)
            {
                media.Source = new Uri(filesList_for_media[selected_index]);
            }
            else
            {
                if (selected_index == 0)
                {
                    selected_index = filesList_for_media.Count;
                }
                media.Source = new Uri(filesList_for_media[selected_index - 1]);
                List.SelectedIndex = copy.IndexOf(filesList_for_media[selected_index - 1]);
                selected_index -= 1;
            }
            
        }

        private void Play_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (play)
            {
                play = false;
                media.Pause();
            }
            else
            {
                play = true;
                media.Play();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Next_song(); 
        }

        private void Again_Click(object sender, RoutedEventArgs e)
        {
            if (play_again)
            {
                play_again= false;
            }
            else
            {
                play_again = true;
            }
        }

        private void Mix_Click(object sender, RoutedEventArgs e)
        {
            if (mixing)
            {
                mixing = false;
                filesList_for_media.Clear();
                filesList_for_media.AddRange(copy);
                selected_index = -1;
            }
            else
            {
                mixing = true;
                for(int i = filesList_for_media.Count - 1; i>=1; i--)
                {
                    int j = random.Next(i + 1);
                    var t = filesList_for_media[j];
                    filesList_for_media[j] = filesList_for_media[i];
                    filesList_for_media[i] = t;
                }
            }
        }

        private void Volume_slide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Volume = Volume_slide.Value / 100;
        }

        private void potok()
        {
            while (play)
            {
                Dispatcher.Invoke(() =>
                {
                    Length_slide.Value = System.Convert.ToDouble(media.Position.Ticks);
                    Now_sec.Text = Convert.ToInt64(Length_slide.Value).ToString();
                    Left_sec.Text = Convert.ToInt64(Length_slide.Maximum - Length_slide.Value).ToString();
                    if(System.Convert.ToDouble(media.Position.Ticks) == Length_slide.Maximum)
                    {
                        Next_song();
                    }
                });
                Thread.Sleep(1000);
            }
        }

        private void Next_song()
        {
            if (play_again)
            {
                media.Source = new Uri(filesList_for_media[selected_index]);
            }
            else
            {
                if (selected_index == filesList_for_media.Count - 1)
                {
                    selected_index = -1;
                }
                media.Source = new Uri(filesList_for_media[selected_index + 1]);
                List.SelectedIndex = copy.IndexOf(filesList_for_media[selected_index + 1]);
                selected_index += 1;
            }
            
        }

        private void media_opened(object sender, RoutedEventArgs e)
        {
            Length_slide.Maximum = media.NaturalDuration.TimeSpan.Ticks;
        }
    }
}

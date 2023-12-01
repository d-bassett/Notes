using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Notes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Window window;
        bool loading;
        Point screen = new Point(2560,1080);
        byte[] opacity;
        byte[] defaultopacity;
        string? currentFile;
        int distance;

        public MainWindow()
        {
            loading = true;
            InitializeComponent();

            richTextBox.Document.Blocks.Clear();
            window = Application.Current.MainWindow;

            if (true)
            {
                StreamReader streamReader = new StreamReader("NotesText.note");
                richTextBox.AppendText(streamReader.ReadToEnd());
                streamReader.Close();
                streamReader.Dispose();
            }

            ImportOptions();

            defaultopacity = new byte[] { opacity[0], opacity[1] };

            loading = false;
        }

        private void ImportOptions()
        {
            if (File.Exists("options.ini"))
            {
                StreamReader streamReader = new StreamReader("options.ini");
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.Split("//")[0];
                    if (!line.Contains(":")) continue;
                    string[] data = line.Split(":", StringSplitOptions.TrimEntries);
                    switch (data[0].ToLower())
                    {
                        case "size":
                            if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[0], out int w))
                            {
                                window.Width = w;
                            }
                            if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[1], out int h))
                            {
                                window.Height = h;
                            }
                            break;
                        case "position":
                            if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[0], out int l))
                            {
                                window.Left = l;
                            }
                            if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[1], out int t))
                            {
                                window.Top = t;
                            }
                            break;
                        case "ontop":
                            if (bool.TryParse(data[1], out bool o))
                            {
                                window.Topmost = o;
                            }
                            break;
                        case "backgroundcolor":
                            {
                                if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[0], out int r))
                                {
                                    if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[1], out int g))
                                    {
                                        if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[2], out int b))
                                        {
                                            if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[3], out int a))
                                            {
                                                richTextBox.Background = new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "textcolor":
                            {
                                if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[0], out int r))
                                {
                                    if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[1], out int g))
                                    {
                                        if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[2], out int b))
                                        {
                                            if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[3], out int a))
                                            {
                                                richTextBox.Foreground = new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "opacity":
                            {
                                opacity = new byte[] { 0, 0 };
                                if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[0], out int op))
                                {
                                    opacity[0] = (byte)op;
                                }
                                if (int.TryParse(data[1].Split(",", StringSplitOptions.TrimEntries)[1], out int opm))
                                {
                                    opacity[1] = (byte)opm;
                                }
                            }
                            break;
                        case "fontsize":
                                if (double.TryParse(data[1], out double f))
                                {
                                    richTextBox.FontSize = f;
                                }
                            break;
                        case "fontfamily":
                            {
                                richTextBox.FontFamily = new FontFamily(data[1]);
                            }
                            break;
                    }
                }
                streamReader.Close();
                streamReader.Dispose();
            }
            else
            {
                window.Width = 400;
                window.Height = 800;
                window.Left = screen.X - window.Width - (distance * 2);
                window.Top = (distance * 4);
                window.Topmost = false;
                opacity = new byte[] { 0, 65 };
                richTextBox.Background = new SolidColorBrush(Color.FromArgb(opacity[0], 0, 0, 0));
                richTextBox.Foreground = new SolidColorBrush(Color.FromArgb(255,255,255,255));
                richTextBox.FontSize = 12;
                ExportOptions();
            }
        }

        private void ExportOptions()
        {
            StreamWriter streamWriter = new StreamWriter("options.ini", false);
            streamWriter.WriteLine(string.Format($"Size: {window.Width}, {window.Height}"));
            streamWriter.WriteLine(string.Format($"Position: {window.Left}, {window.Top}"));
            streamWriter.WriteLine(string.Format($"OnTop: {window.Topmost}"));
            streamWriter.WriteLine(string.Format($"Opacity: {opacity[0]}, {opacity[1]}"));
            streamWriter.WriteLine(string.Format($"BackgroundColor: {((SolidColorBrush)richTextBox.Background).Color.R}, {((SolidColorBrush)richTextBox.Background).Color.G}, {((SolidColorBrush)richTextBox.Background).Color.B}, {((SolidColorBrush)richTextBox.Background).Color.A}"));
            streamWriter.WriteLine(string.Format($"TextColor: {((SolidColorBrush)richTextBox.Foreground).Color.R}, {((SolidColorBrush)richTextBox.Foreground).Color.G}, {((SolidColorBrush)richTextBox.Foreground).Color.B}, {((SolidColorBrush)richTextBox.Foreground).Color.A}"));
            streamWriter.WriteLine(string.Format($"FontSize: {richTextBox.FontSize}"));
            streamWriter.WriteLine(string.Format($"FontFamily: {richTextBox.FontFamily}"));
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }

        private void Notes_MouseEnter(object sender, MouseEventArgs e)
        {
            richTextBox.Background = new SolidColorBrush(Color.FromArgb(opacity[1],0,0,0));
        }

        private void Notes_MouseLeave(object sender, MouseEventArgs e)
        {
            richTextBox.Background = new SolidColorBrush(Color.FromArgb(opacity[0], 0, 0, 0));
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (loading) return;
            //StreamWriter streamWriter = new StreamWriter("NotesText.note", false);
            //streamWriter.Write(new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text);
            //streamWriter.Flush();
            //streamWriter.Close();
            //streamWriter.Dispose();
        }

        private void Notes_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (loading) return;
            ExportOptions();
        }

        private void Notes_LocationChanged(object sender, EventArgs e)
        {
            if (loading) return;
            ExportOptions();
        }

        private void Notes_KeyDown(object sender, KeyEventArgs e)
        {

            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl);
            bool shift = Keyboard.IsKeyDown(Key.LeftShift);

            if (e.Key == Key.F1)
            {
                window.Topmost = !window.Topmost;
            }

            if (e.Key == Key.F2)
            {
                ResetPosition();
                ExportOptions();
            }

            if (e.Key == Key.F3)
            {
                opacity[0] = (opacity[0] == (byte)defaultopacity[0]) ? (byte)defaultopacity[1] : (byte)defaultopacity[0];
                opacity[1] = (opacity[1] == (byte)defaultopacity[1]) ? (byte)255 : (byte)defaultopacity[1];
                richTextBox.Background = new SolidColorBrush(Color.FromArgb(window.IsMouseOver ? opacity[1] : opacity[0], 0, 0, 0));
            }

            if (shift)
            {
                if (e.Key == Key.F5)
                {
                    window.Width = Math.Max(0, window.Width - distance);
                }
                if (e.Key == Key.F6)
                {
                    window.Width = Math.Min(screen.X - window.Left, window.Width + distance);
                }
                if (e.Key == Key.F7)
                {
                    window.Height = Math.Max(0, window.Height - distance);
                }
                if (e.Key == Key.F8)
                {
                    window.Height = Math.Min(screen.Y - window.Top, window.Height + distance);
                }
            }
            else
            {
                if (e.Key == Key.F5)
                {
                    window.Left = ctrl ? Math.Max(0, window.Left - distance) : 0;
                }
                if (e.Key == Key.F6)
                {
                    window.Top = ctrl ? Math.Min(screen.Y - window.Height, window.Top + distance) : (screen.Y - window.Height);
                }
                if (e.Key == Key.F7)
                {
                    window.Top = ctrl ? Math.Max(0, window.Top - distance) : 0;
                }
                if (e.Key == Key.F8)
                {
                    window.Left = ctrl ? Math.Min(screen.X - window.Width, window.Left + distance) : (screen.X - window.Width);
                }
            }
            

            if (e.Key == Key.S && ctrl)
            {

                if (string.IsNullOrWhiteSpace(currentFile))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    bool? result = saveFileDialog.ShowDialog();
                    if (result.HasValue ? result.Value : false)
                    {
                        StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false);
                        streamWriter.Write(new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text);
                        streamWriter.Flush();
                        streamWriter.Close();
                        streamWriter.Dispose();
                        currentFile = saveFileDialog.FileName;
                    }
                } else
                {
                    StreamWriter streamWriter = new StreamWriter(currentFile, false);
                    streamWriter.Write(new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text);
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }

            if (e.Key == Key.O && ctrl)
            {
                if (string.IsNullOrWhiteSpace(currentFile))
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    bool? result = openFileDialog.ShowDialog();
                    if (result.HasValue ? result.Value : false)
                    {
                        StreamReader streamReader = new StreamReader(openFileDialog.FileName);
                        richTextBox.Document.Blocks.Clear();
                        richTextBox.AppendText(streamReader.ReadToEnd());
                        streamReader.Close();
                        streamReader.Dispose();
                        currentFile = openFileDialog.FileName;
                    }
                }
                else
                {
                    StreamReader streamReader = new StreamReader(currentFile);
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.AppendText(streamReader.ReadToEnd());
                    streamReader.Close();
                    streamReader.Dispose();
                }
            }
        }

        void ResetPosition()
        {
            window.Top = 0;
            window.Left = 0;
        }
    }
}

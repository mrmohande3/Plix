using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlixP.Renders
{
    public class ImageCacher : Image
    {
        private string BaseRepos = string.Format("{0}\\PlixMovieCacheFolder\\", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        private List<FileInfo> FileInfos;
        private bool Sourced = false;


        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(string), typeof(ImageCacher), 
                new PropertyMetadata("", new PropertyChangedCallback(OnSetTextChanged)));

        private static void OnSetTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageC = d as ImageCacher;
            var source = e.NewValue as string;
            if (source != null)
            {
                var name = source.ToString().Split('/').Last();
                var image = imageC.FileInfos.FirstOrDefault(f => f.Name == name);
                if (image != null)
                {
                    imageC.Sourced = true;
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(Path.Combine(image.DirectoryName, image.Name), UriKind.Absolute);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    imageC.Source = src;
                }
                else
                {
                    try
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                if (source != "N/A")
                                {
                                    byte[] data = webClient.DownloadData(source.ToString());
                                    File.WriteAllBytes(Path.Combine(imageC.BaseRepos, name), data);
                                    imageC.Source = new BitmapImage(new Uri(Path.Combine(imageC.BaseRepos, name)));
                                    imageC.Sourced = true;
                                }
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }


            }
        }


        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public ImageCacher()
        {
            var directoryInfo = new DirectoryInfo(BaseRepos);
            if (!Directory.Exists(BaseRepos))
            {
                Directory.CreateDirectory(BaseRepos);
            }
            FileInfos = directoryInfo.GetFiles().ToList();
            if (FileInfos == null)
                FileInfos = new List<FileInfo>();
        }
    }
}

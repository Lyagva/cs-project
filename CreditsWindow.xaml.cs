using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace intruder
{
    /// <summary>
    /// Логика взаимодействия для CreditsWindow.xaml
    /// </summary>
    public partial class CreditsWindow : Window
    {
        MediaPlayer audio = new MediaPlayer();
        public CreditsWindow()
        {
            InitializeComponent();
        }
        bool transcorrector = false; // чтобы не багалась анимация выкатывания, без него, при молейшем движении по границе подвижного эллемента - проигрывалась анимация повторно
        private void spec__down(object sender, MouseEventArgs e)
        {
            audio.Open(new Uri("music/put_down.mp3", UriKind.Relative)); // мшк фреде
            if (transcorrector == false)
            {
                TranslateTransform trans = new TranslateTransform();
                tab.RenderTransform = trans;
                DoubleAnimation mainanim = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(3));
                trans.BeginAnimation(TranslateTransform.YProperty, mainanim);

                var opacity_tab = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = new Duration(TimeSpan.FromSeconds(1.5))
                };
                tab.BeginAnimation(OpacityProperty, opacity_tab);

                TranslateTransform fok = new TranslateTransform();
                secret.RenderTransform = fok;
                DoubleAnimation sideanim = new DoubleAnimation(0, 113, TimeSpan.FromSeconds(1.5));
                fok.BeginAnimation(TranslateTransform.YProperty, sideanim);

                var opacity_secret = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(2.5))
                };
                secret.BeginAnimation(OpacityProperty, opacity_secret);

                transcorrector = true;
            }
            audio.Stop();
            audio.Play();
        }
        private void spec__up(object sender, RoutedEventArgs e)
        {
            audio.Open(new Uri("music/put_down.mp3", UriKind.Relative)); // мшк фреде
            if (transcorrector == true)
            {
                var opacity_secret = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = new Duration(TimeSpan.FromSeconds(1.5))
                };
                secret.BeginAnimation(OpacityProperty, opacity_secret);

                TranslateTransform trans = new TranslateTransform();
                tab.RenderTransform = trans;
                DoubleAnimation mainanim = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(1.5));
                trans.BeginAnimation(TranslateTransform.YProperty, mainanim);

                var opacity_tab = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(3))
                };
                tab.BeginAnimation(OpacityProperty, opacity_tab);

                TranslateTransform fok = new TranslateTransform();
                secret.RenderTransform = fok;
                DoubleAnimation sideanim = new DoubleAnimation(113, 0, TimeSpan.FromSeconds(2.5));
                fok.BeginAnimation(TranslateTransform.YProperty, sideanim);

                transcorrector = false;
            }
            audio.Stop();
            audio.Play();
        }

        // саунд при наведении на иконки
        private void audioforsite(object sender, MouseEventArgs e)
        {
            audio.Open(new Uri("music/site_cover.mp3", UriKind.Relative));
            audio.Stop();
            audio.Play();
        }

        // ссылки на авторов
        private void site__artemYouTube(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://youtube.com/channel/UC4ogHkBFO1afHQ4rR2TN-hA");
        }
        private void site__yuriyDiscord(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/3F2Ms2bAwB");
        }
        private void site__yarikVK(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/id519072952");
        }
    }
}

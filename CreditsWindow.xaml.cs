using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace intruder
{
    /// <summary>
    /// Логика взаимодействия для CreditsWindow.xaml
    /// </summary>
    public partial class CreditsWindow : Window
    {
        public CreditsWindow()
        {
            InitializeComponent();
        }
        bool transcorrector = false; // чтобы не багалась анимация выкатывания, без него, при молейшем движении по границе подвижного эллемента - проигрывалась анимация повторно
        private void spec__down(object sender, MouseEventArgs e)
        {
            if (transcorrector == false)
            {
                TranslateTransform trans = new TranslateTransform();
                tab.RenderTransform = trans;
                DoubleAnimation mainanim = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(3));
                trans.BeginAnimation(TranslateTransform.YProperty, mainanim);

                TranslateTransform fok = new TranslateTransform();
                secret.RenderTransform = fok;
                DoubleAnimation sideanim = new DoubleAnimation(0, 113, TimeSpan.FromSeconds(1.5));
                fok.BeginAnimation(TranslateTransform.YProperty, sideanim);

                transcorrector = true;
            }
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

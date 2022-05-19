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

namespace CMD
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
        private void spec__down(object sender, MouseEventArgs e)
        {
            TranslateTransform trans = new TranslateTransform();
            tab.RenderTransform = trans;
            DoubleAnimation mainanim = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(3));
            trans.BeginAnimation(TranslateTransform.YProperty, mainanim);

            TranslateTransform fok = new TranslateTransform();
            secret.RenderTransform = fok;
            DoubleAnimation sideanim = new DoubleAnimation(0, 113, TimeSpan.FromSeconds(1.5));
            fok.BeginAnimation(TranslateTransform.YProperty, sideanim);
        }
    }
}

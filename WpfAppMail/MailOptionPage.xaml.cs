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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppMail
{
    /// <summary>
    /// Логика взаимодействия для MailOptionPage.xaml
    /// </summary>
    public partial class MailOptionPage : Page
    {
        private readonly MailOptions mailOptions;

        public MailOptionPage(MailOptions mailOptions)
        {
            InitializeComponent();
            DataContext = mailOptions;
            this.mailOptions = mailOptions;
        }

        private async void Send(object sender, RoutedEventArgs e)
        {
            MailService.Setup(mailOptions);

            var result = await MailService.SendEmailAsync("stenly87@bk.ru", "subject", "test text");
            if (result.Success)
                MessageBox.Show("Сообщение отправлено");
            else
                MessageBox.Show(result.Error);
        }
    }
}

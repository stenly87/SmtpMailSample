using System.Net;
using System.Net.Mail;
using System.Text;

namespace WpfAppMail
{
    public static class MailService
    {
        private static MailOptions mailOptions;

        public static void Setup(MailOptions mailOptions)
        {
            MailService.mailOptions = mailOptions;
        }

        private static SmtpClient CreateSmtp()
        {
            var smtpClient = new SmtpClient();
            smtpClient.Host = mailOptions.SmtpServer;
            smtpClient.Port = mailOptions.SmtpPort;
            smtpClient.EnableSsl = mailOptions.EnableSsl;
            smtpClient.Credentials = new NetworkCredential(mailOptions.From, mailOptions.Password);
            smtpClient.SendCompleted += SmtpClient_SendCompleted;
            return smtpClient;
        }

        private static void SmtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SmtpResult? smtpResult = e.UserState as SmtpResult;
            SmtpClient? smtpClient = sender as SmtpClient;
            if (smtpResult == null)
                return;

            if (e.Error != null)
            {
                smtpResult.Success = false;
                smtpResult.Error = e.Error.Message;
            }
            else
                smtpResult.Success = true;
            try
            {
                if (smtpClient != null)
                {
                    smtpClient.SendCompleted -= SmtpClient_SendCompleted;
                    smtpClient.Dispose();
                }
            }
            catch { }
        }

        public static async Task<SmtpResult> SendEmailAsync(string target, string title, string body)
        {
            SmtpResult callback = new SmtpResult();
            try
            {
                await Task.Run(() =>
                {
                    var task = new TaskCompletionSource(callback);
                    var smtpClient = CreateSmtp();
                    MailMessage message = GenerateMessage(target, title, body);
                    smtpClient.SendAsync(message, callback);
                    return task.Task;

                });
                return callback;
            }
            catch (Exception e)
            {
                callback.Error = e.Message;
                return callback;
            }
        }

        private static MailMessage GenerateMessage(string target, string title, string body)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(body);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(mailOptions.Footer);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(mailOptions.From);
            message.Body = stringBuilder.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = title;
            message.SubjectEncoding = Encoding.UTF8;
            message.To.Add(new MailAddress(target));
            return message;
        }

        public class SmtpResult
        {
            public bool Success { get; internal set; }
            public string Error { get; internal set; }
        }
    }
}

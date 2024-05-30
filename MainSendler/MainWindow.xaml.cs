using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace MailSender_PD_322
{
    public partial class MainWindow : Window
    {
        string server = "smtp.gmail.com";
        int port = 587;
        private string attachmentFilePath = "";

        string username = "";
        string password = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Please fill all required fields");
                return;
            }

            MailMessage message = new MailMessage(fromObject.Text, toObject.Text, subject.Text, bodyObject.Text);
            SmtpClient client = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            message.Priority = importantCheckBox.IsChecked == true ? MailPriority.High : MailPriority.Normal;

            try
            {
                if (!string.IsNullOrWhiteSpace(attachmentFilePath))
                {
                    message.Attachments.Add(new Attachment(attachmentFilePath));
                }

                client.SendCompleted += Client_SendCompleted;
                client.SendAsync(message, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(toObject.Text) &&
                   !string.IsNullOrWhiteSpace(subject.Text) &&
                   !string.IsNullOrWhiteSpace(fromObject.Text);
        }

        private void AttachFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                attachmentFilePath = openFileDialog.FileName;
            }
        }

        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show($"An error occurred: {e.Error.Message}");
                Console.WriteLine($"An error occurred: {e.Error.Message}");
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("The send was cancelled.");
                Console.WriteLine("The send was cancelled.");
            }
            else
            {
                var state = (MailMessage)e.UserState!;
                MessageBox.Show($"Message was sent! Subject: {state.Subject}");
                Console.WriteLine($"Message was sent! Subject: {state.Subject}");
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginTextBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBox.Show("Please enter both username and password");
                return;
            }

            username = loginTextBox.Text;
            password = passwordBox.Password;

            // Виконуємо перевірку автентифікації
            if (AuthenticateSmtpClient(username, password))
            {
                MessageBox.Show("Logged in successfully");
            }
            else
            {
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private bool AuthenticateSmtpClient(string username, string password)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(server, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                })
                {
                    // Відправляємо NOOP команду для перевірки підключення
                    client.Send(new MailMessage(username, username, "Test", "Test")
                    {
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        SubjectEncoding = System.Text.Encoding.UTF8
                    });
                }
                return true; // Якщо не виникло виключень, то автентифікація пройшла успішно
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP authentication failed: {smtpEx.StatusCode} - {smtpEx.Message}");
                return false; // Якщо виникло виключення, то автентифікація не пройшла
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception: {ex.Message}");
                return false; // Якщо виникло виключення, то автентифікація не пройшла
            }
        }

    }
}

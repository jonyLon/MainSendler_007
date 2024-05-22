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
        int port = 3000;
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
            SmtpClient client = new SmtpClient(server, port);
            client.Credentials = new NetworkCredential(username, password);

            message.Priority = importantCheckBox.IsChecked == true ? MailPriority.High : MailPriority.Normal;

            try
            {
                client.EnableSsl = true;

                if (!string.IsNullOrWhiteSpace(attachmentFilePath))
                {
                    message.Attachments.Add(new Attachment(attachmentFilePath));
                }

                client.SendCompleted += Client_SendCompleted;
                client.SendAsync(message, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred :: {ex.Message}");
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
            var state = (MailMessage)e.UserState!;
            MessageBox.Show($"Message was sent! Subject :: {state.Subject}");
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

            MessageBox.Show("Logged in successfully");
        }
    }
}

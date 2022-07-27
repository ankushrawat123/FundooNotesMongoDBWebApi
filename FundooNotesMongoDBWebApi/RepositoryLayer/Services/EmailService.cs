using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RepositoryLayer.Services
{
    public class EmailService
    {
        public static void SendEmail(string Email, string token)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential("ankushrwt61@gmail.com", "cylwoiebgvfcpojf");

                MailMessage msgObj = new MailMessage();
                msgObj.To.Add(Email);
                msgObj.From = new MailAddress("ankushrwt61@gmail.com");
                msgObj.Subject = "Password Reset Link";
                msgObj.Body = $"www.AnkushBookStore.com/reset-password/{token}";
                smtpClient.Send(msgObj);

            }

        }
    }
}

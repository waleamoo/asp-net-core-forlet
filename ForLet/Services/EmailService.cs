using ForLet.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ForLet.Services
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"EmailTemplate/{0}.html";
        private readonly SMTPConfigModel _smtpConfig;

        public async Task SendTestEmail(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Hello {{ UserName }}, From ForLet", userEmailOptions.PlaceHolders);

            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("TestEmail"), userEmailOptions.PlaceHolders);

            await SendEmail(userEmailOptions);
        }

        public async Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Hello {{ UserName }}, Confirm your email id.", userEmailOptions.PlaceHolders);

            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("EmailConfirm"), userEmailOptions.PlaceHolders);

            await SendEmail(userEmailOptions);
        }

        // email the user for the forgot password 
        public async Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Hello {{ UserName }}, reset your password.", userEmailOptions.PlaceHolders);

            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("ForgotPassword"), userEmailOptions.PlaceHolders);

            await SendEmail(userEmailOptions);
        }

        public EmailService(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        // helper method to send all email
        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new System.Net.Mail.MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                IsBodyHtml = _smtpConfig.IsBodyHTML
            };

            foreach (var toEmail in userEmailOptions.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = _smtpConfig.EnableSSL,
                Credentials = new System.Net.NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password)
            };


            mail.BodyEncoding = Encoding.Default;

            await smtpClient.SendMailAsync(mail);

            smtpClient.Dispose();


            //var client = new System.Net.Mail.SmtpClient(_smtpConfig.Host, _smtpConfig.Port)
            //{
            //    Credentials = new System.Net.NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password),
            //    EnableSsl = true
            // };

            //foreach (var toEMail in userEmailOptions.ToEmails)
            //{
            //    await client.SendMailAsync(_smtpConfig.SenderAddress, toEMail, userEmailOptions.Subject, userEmailOptions.Body);
            //}


        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }
            return text;
        }
    }
}

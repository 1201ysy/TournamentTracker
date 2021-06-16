using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace TrackerLibrary
{
    public static class EmailLogic
    {
        /// <summary>
        /// Send Email using only To, Subject, Body parameters
        /// </summary>
        /// <param name="toAddr"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmail(string toAddr, string subject, string body)
        {
            SendEmail(new List<string> { toAddr }, new List<string>(), subject, body );

        }

        /// <summary>
        /// Send Emails using all the below paramaters 
        /// </summary>
        /// <param name="toAddr"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmail(List<string> toAddr, List<string> bcc, string subject, string body)
        {

            MailAddress fromMailAddr = new MailAddress(GlobalConfig.AppKeyLookup("senderEmail"), "Tournament Tracker");

            MailMessage mail = new MailMessage();
            foreach (string emails in toAddr)
            {
                mail.To.Add(emails);
            }
            mail.From = fromMailAddr;
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Bcc = bcc;

            SmtpClient client = new SmtpClient();

            client.Send(mail);

        }
    }
}

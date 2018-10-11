using System;
using System.Net.Mail;
using SMARTscan_DataProcessor.Data;

namespace SMARTscan_DataProcessor.Controller
{
    public class MailSender
    {
        public static void SendNotification(string jobname)
        {
            string carnellSMTP = Global.SmtpServer;
            SmtpClient SmtpServer = new SmtpClient(carnellSMTP);
            string time = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Global.SmtpFromTo)
            };

            //mail.To.Add(Global.user1);
            //mail.To.Add(Global.user2);
            mail.To.Add(Global.Hao);
            mail.Subject = "SMARTscan Automation Email - Status: Ready for QA";
            mail.Body = $"<h3>This is an automated email, please do not reply.</h3><p>Your job has been successfully processed and need you to implement data QA.</p><p> Date: {time} </p><p> Job Number: {jobname} </p><p> Thank you for uploading the data!</p> ";
            mail.IsBodyHtml = true;

            SmtpServer.Port = 25;
            SmtpServer.Credentials = new System.Net.NetworkCredential("hao.ye@carnellgroup.co.uk", "Carnell2019");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
    }
}

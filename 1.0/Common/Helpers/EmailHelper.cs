using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Common.Extensions;

namespace Common.Helpers
{
    public class EmailHelper
    {
        public List<string> ToAddresses { get; set; }
        public List<string> CcAddresses { get; set; }
        public List<string> BccAddresses { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool HTML { get; set; }
        public bool SSL { get; set; }
        public List<EmailHelperAttachment> Attachments { get; set; }
        public bool ZipAttachments { get; set; }
        public string ZipFileName { get; set; }
        private MemoryStream msZip { get; set; }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigHelper.SmtpSection.Network.Host) || string.IsNullOrEmpty(ConfigHelper.SmtpSection.Network.UserName) || string.IsNullOrEmpty(ConfigHelper.SmtpSection.Network.Password) || ToAddresses.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        public EmailHelper()
        {
            Attachments = new List<EmailHelperAttachment>();
            ToAddresses = CcAddresses = BccAddresses = new List<string>();
        }

        public void Send()
        {
            if (ConfigHelper.SmtpSection.Network.Port == 465)
            {
                sendSSL();
                return;
            }

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(!string.IsNullOrWhiteSpace(FromAddress) ? FromAddress : ConfigHelper.SmtpSection.From );
            mailMessage.Subject = Subject;
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = HTML;

            if (ConfigHelper.AppSettings.Environment == ConfigHelper.Environment.PROD)
            {
                if (ToAddresses.IsNullOrEmpty())
                    mailMessage.To.Add(ConfigHelper.AppSettings.SmtpDefaultFrom);
                else
                    ToAddresses.ForEach(email => mailMessage.To.Add(email));
                CcAddresses.ForEach(email => mailMessage.CC.Add(email));
                BccAddresses.ForEach(email => mailMessage.Bcc.Add(email));
            }
            else
            {
                mailMessage.To.Add(ConfigHelper.AppSettings.SmtpDefaultFrom);
                var emails = string.Format("Original Email List:<br/>To: {0}<br/><br/>{1}<br/><br/>{2}"
                    , string.Join(", ", ToAddresses.ToArray())
                    , string.Join(", ", CcAddresses.ToArray())
                    , string.Join(", ", BccAddresses.ToArray()));
                Body = Body.Insert(0, emails);
            }

            if (Attachments != null && Attachments.Count > 0)
            {
                if (ZipAttachments)
                {
                    byte[] zipFile = GenerateZip();
                    if (zipFile != null)
                    {
                        string fileName = !string.IsNullOrEmpty(ZipFileName) ? ZipFileName : DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(zipFile), fileName));
                    }
                }
                else
                    attachFiles(mailMessage);
            }

            SmtpClient smtpClient = new SmtpClient(ConfigHelper.SmtpSection.Network.Host, ConfigHelper.SmtpSection.Network.Port);
            smtpClient.Credentials = new System.Net.NetworkCredential(ConfigHelper.SmtpSection.Network.UserName, ConfigHelper.SmtpSection.Network.Password);
            smtpClient.Send(mailMessage);

            if (msZip != null)
            {
                msZip.Close();
                msZip.Dispose();
            }
        }

        private void sendSSL()
        {
            System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
            mailMessage.From = !string.IsNullOrWhiteSpace(FromAddress) ? FromAddress : ConfigHelper.SmtpSection.From;
            mailMessage.Subject = Subject;
            mailMessage.Body = Body;
            mailMessage.BodyFormat = System.Web.Mail.MailFormat.Html;

            if (ConfigHelper.AppSettings.Environment == ConfigHelper.Environment.PROD)
            {
                if (ToAddresses.IsNullOrEmpty())
                    mailMessage.To = ConfigHelper.AppSettings.SmtpDefaultFrom;
                else
                    mailMessage.To = string.Join(",", ToAddresses.ToArray());
                mailMessage.Cc = string.Join(",", CcAddresses.ToArray());
                mailMessage.Bcc = string.Join(",", BccAddresses.ToArray());
            }
            else
            {
                mailMessage.To = ConfigHelper.AppSettings.SmtpDefaultFrom;
                var emails = string.Format("Original Email List:<br/>To: {0}<br/><br/>Cc: {1}<br/><br/>Bcc: {2}"
                    , string.Join(", ", ToAddresses.ToArray())
                    , string.Join(", ", CcAddresses.ToArray())
                    , string.Join(", ", BccAddresses.ToArray()));
                Body = Body.Insert(0, emails);
            }

            if (Attachments != null && Attachments.Count > 0)
            {
                if (ZipAttachments)
                {
                    byte[] zipFile = GenerateZip();
                    if (zipFile != null)
                    {
                        string fileName = !string.IsNullOrEmpty(ZipFileName) ? ZipFileName : DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(zipFile), fileName));
                    }
                }
                else
                    attachFiles(mailMessage);
            }


            mailMessage.Fields[SMTP_SERVER] = ConfigHelper.SmtpSection.Network.Host;
            mailMessage.Fields[SMTP_SERVER_PORT] = ConfigHelper.SmtpSection.Network.Port;
            mailMessage.Fields[SEND_USING] = 2;
            mailMessage.Fields[SMTP_USE_SSL] = true;
            mailMessage.Fields[SMTP_AUTHENTICATE] = 1;
            mailMessage.Fields[SEND_USERNAME] = ConfigHelper.SmtpSection.Network.UserName;
            mailMessage.Fields[SEND_PASSWORD] = ConfigHelper.SmtpSection.Network.Password;

            System.Web.Mail.SmtpMail.Send(mailMessage);
        }

        private const string SMTP_SERVER = "http://schemas.microsoft.com/cdo/configuration/smtpserver";
        private const string SMTP_SERVER_PORT = "http://schemas.microsoft.com/cdo/configuration/smtpserverport";
        private const string SEND_USING = "http://schemas.microsoft.com/cdo/configuration/sendusing";
        private const string SMTP_USE_SSL = "http://schemas.microsoft.com/cdo/configuration/smtpusessl";
        private const string SMTP_AUTHENTICATE = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
        private const string SEND_USERNAME = "http://schemas.microsoft.com/cdo/configuration/sendusername";
        private const string SEND_PASSWORD = "http://schemas.microsoft.com/cdo/configuration/sendpassword";

        private byte[] GenerateZip()
        {
            //if (Attachments != null && Attachments.Count > 0)
            //{
            //    using (msZip = new MemoryStream())
            //    {
            //        using (ZipOutputStream zipStream = new ZipOutputStream(msZip))
            //        {
            //            Crc32 crc = new Crc32();
            //            zipStream.SetLevel(3);

            //            foreach (EmailFactoryAttachment attachment in Attachments)
            //            {
            //                ZipEntry zipEntry = new ZipEntry(attachment.FileName);
            //                zipEntry.DateTime = DateTime.UtcNow;
            //                crc.Reset();
            //                crc.Update(attachment.FileContents);
            //                zipEntry.Crc = crc.Value;
            //                zipEntry.Size = attachment.FileContents.Length;
            //                zipStream.PutNextEntry(zipEntry);
            //                zipStream.Write(attachment.FileContents, 0, attachment.FileContents.Length);
            //                zipStream.CloseEntry();
            //            }

            //            msZip.Position = 0;
            //            return msZip.ToArray();
            //        }
            //    }
            //}

            return null;
        }

        public void AddAttachment(string fileName, byte[] fileContents)
        {
            this.Attachments.Add(new EmailHelperAttachment { FileName = fileName, FileContents = fileContents });
        }

        private void attachFiles(MailMessage mailMessage)
        {
            if (Attachments != null && Attachments.Count > 0)
            {
                foreach (EmailHelperAttachment attachment in Attachments)
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.FileContents), attachment.FileName));
            }
        }

        private void attachFiles(System.Web.Mail.MailMessage mailMessage)
        {
            if (Attachments != null && Attachments.Count > 0)
            {
                foreach (EmailHelperAttachment attachment in Attachments)
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.FileContents), attachment.FileName));
            }
        }
    }

    public class EmailHelperAttachment
    {
        public string FileName { get; set; }
        public byte[] FileContents { get; set; }

        public EmailHelperAttachment() { }
    }
}
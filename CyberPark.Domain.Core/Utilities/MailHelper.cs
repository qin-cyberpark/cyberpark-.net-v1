using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CyberPark.Domain.Core;
namespace CyberPark.Domain.Utilities
{

    public class MailHelper
    {
        private static SmtpClient _client;
        private static MailAddress _fromAddress;
        private static object _locker = new object();
        static MailHelper()
        {
            _client = new SmtpClient(SysConfig.Instance.MailSenderServer, SysConfig.Instance.MailSenderPort);
            _client.Credentials = new System.Net.NetworkCredential(SysConfig.Instance.MailSenderUsername, 
                                                                    SysConfig.Instance.MailSenderPassword);
            _client.EnableSsl = SysConfig.Instance.MailSenderEnableSSL;
            _client.Timeout = SysConfig.Instance.MailSenderTimeout;

            _fromAddress = new MailAddress(SysConfig.Instance.MailSenderAddress, 
                                            SysConfig.Instance.MailSenderDisplayName);
        }

        public static bool Send(string toAddress, string subject, string body, string[] attachments, ref string msg)
        {
            lock (_locker) {
                // Specify the message content.
                MailMessage message = new MailMessage(_fromAddress, new MailAddress(toAddress));
                message.Subject = subject;
                message.Body = body;
                foreach(var s in attachments)
                {
                    if (System.IO.File.Exists(s)) {
                        message.Attachments.Add(new Attachment(s));
                    }
                }
                try
                {
                    _client.Send(message);
                    return true;
                }
                catch(Exception ex)
                {
                    msg = ex.Message + "," + ex.InnerException?.Message;
                    return false;
                }
                finally
                {
                    // Clean up.
                    message.Dispose();
                }
            }
        }

        ~MailHelper()
        {
            _client.Dispose();
        }
    }
}

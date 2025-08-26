using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using ArchiveNow.Actions.Core.Contexts;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;

namespace ArchiveNow.Actions.Core
{
    public class SendToMailboxAction : AfterFinishedActionBase
    {
        private const string DefaultSubject = "[ArchiveNow]";
        private const string DefaultBody = "The file was automatically sent by ArchiveNow!";
        private const int DefaultPort = 25;

        private readonly string _host;
        private readonly int _port;
        private readonly NetworkCredential _cridentials;
        private readonly string _subject;

        private readonly MailAddress _recipientAddress;
        private readonly MailAddress _senderAddress;

        private static readonly Lazy<NetworkCredential> DefaultCredential =
            new Lazy<NetworkCredential>(() => CredentialCache.DefaultNetworkCredentials);

        public override string Description => "Sending to the mailbox...";

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="mailContext"></param>
        public SendToMailboxAction(MailContext mailContext)
            : base(precedence: 8)
        {
            _host = mailContext.Host;
            _port = mailContext.Port ?? DefaultPort;

            _cridentials = string.IsNullOrWhiteSpace(mailContext.UserName)
                ? DefaultCredential.Value
                : new NetworkCredential(mailContext.UserName, mailContext.Password);

            _senderAddress = new MailAddress(mailContext.Sender);
            _recipientAddress = new MailAddress(mailContext.Recipient);

            _subject = mailContext.Subject ?? DefaultSubject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var hasError = false;
            var message = string.Empty;

            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = _host,
                    Port = _port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = _cridentials
                };

                var mailMessage = new MailMessage(_senderAddress, _recipientAddress)
                {
                    Subject = _subject,
                    Body = DefaultBody
                };

                var fileName = Path.GetFileName(context.InputPath);
                if (fileName == null)
                {
                    throw new NullReferenceException("fileName");
                }

                var attachmentName = fileName.ReplaceLast(".", "_");

                var fileStream = new FileStream(context.InputPath, FileMode.Open);
                mailMessage.Attachments.Add(new Attachment(fileStream, attachmentName));

                // TEST
                //smtpClient.Send(mailMessage);

                mailMessage.Dispose();
            }
            catch (Exception ex)
            {
                hasError = true;
                message = ex.Message;
            }

            return new AfterFinishedActionResult(hasError.Not(), context.InputPath, message);
        }
    }
}

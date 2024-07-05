using System;
using System.Collections.Generic;

using ArchiveNow.Actions.Core.Contexts;
using ArchiveNow.Utils.Windows;

namespace ArchiveNow.Actions.Core
{
    public class AfterFinishedActionFactory
    {
        public static IAfterFinishedAction Build(string name, IDictionary<string, object> parameters)
        {
            Func<IAfterFinishedAction> creator;

            switch (name)
            {
                case "SendToMailBox":
                    var mailContext = new MailContext
                    {
                        Host = GetValue<string>(parameters, "Host"),
                        Port = GetValue<int?>(parameters, "Port"),
                        UserName = GetValue<string>(parameters, "UserName"),
                        Password = GetValue<string>(parameters, "Password"),
                        Recipient = GetValue<string>(parameters, "Recipient"),
                        Sender = GetValue<string>(parameters, "Sender"),
                        Subject = GetValue<string>(parameters, "Subject")
                    };

                    creator = (() => new SendToMailBoxAction(mailContext));
                    break;

                case "UploadToGoogleDrive":
                    return NullAction.Instance;
                    //throw new NotImplementedException("UploadToGoogleDrive");

                    //var googleDriveContext = new GoogleDriveContext
                    //{
                    //    SecretKeyFilePath = GetValue<string>(parameters, "SecretKeyFilePath"),
                    //    DestinationFolderId = GetValue<string>(parameters, "DestinationFolderId")
                    //};

                    //creator = (() => new UploadToGoogleDriveAction(googleDriveContext));
                    //break;

                case "SetClipboard":
                    creator = (() => new SetClipboardAction(new ClipboardService()));
                    break;

                case "MoveToDirectory":
                    creator = (() => new MoveToDirectoryAction((string)parameters["Path"]));
                    break;

                case "Delete":
                    creator = (() => new DeleteAction());
                    break;

                case "Encrypt":
                    creator = (() => new EncryptAction((string)parameters["PublicKeyFilePath"]));
                    break;

                case "Test":
                    creator = (() => new TestAction());
                    break;

                default:
                    throw new NotSupportedException($"The action '{name}' is not supported!");
            }

            return creator();
        }

        private static T GetValue<T>(IDictionary<string, object> parameters, string key)
        {
            if (parameters.ContainsKey(key))
            {
                return (T) parameters[key];
            }

            return default;
        }
    }
}

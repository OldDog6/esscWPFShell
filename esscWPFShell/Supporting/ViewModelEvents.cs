using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esscWPFShell.Supporting
{
    /// <summary>
    /// Basic notification between vm and vw
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        public String Message { get; protected set; }
    }

    public class DialogClosed : EventArgs
    {
        public DialogClosed(bool value)
        {
            OKButtonPressed = value;
        }

        public bool OKButtonPressed { get; set; }
    }

    public class GenericEventArgs : EventArgs
    {
        public GenericEventArgs(string s)
        {
            message = s;
        }

        public GenericEventArgs()
        {
            message = String.Empty;
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

}

using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masters_2024_MSS_521
{
    /// <summary>
    /// The emperor is most displeased.For all his logging, it has caused strife.
    /// </summary>
    /// <remarks>
    /// If you are attempting to view these logs and you are using this own your own personal VC4 install
    /// (meaning you have access to the Linux CLI) enter the following command at the CLI to view entries
    /// in the log: "sudo grep “SimplSharpPro” /var/log/messages"
    /// </remarks>
    public static class LogusMaximus
    {
        /// <summary>
        /// Arbitor for messages everywhere to find the error log.
        /// </summary>
        /// <remarks>
        /// If you are attempting to view these logs and you are using this own your own personal VC4 install
        /// (meaning you have access to the Linux CLI) enter the following command at the CLI to view entries
        /// in the log: "sudo grep “SimplSharpPro” /var/log/messages"
        /// </remarks>
        /// <param name="header">Log header that identifies the source of the log</param>
        /// <param name="msg1">Message 1 to be inserted as part of the log entry</param>
        /// <param name="msg2">Message 2 to be inserted as part of the log entry</param>
        /// <param name="type">0 for notice or 1 for error</param>
        public static void LogusDecimusMaximus(string header, string msg1, string msg2, uint type)
        {
            switch (type)
            {
                case 0:
                    {
                        ErrorLog.Notice(string.Format(header +
                            "{0} {1}",
                                msg1, msg2));
                        break;
                    }
                case 1:
                    {
                        ErrorLog.Error(string.Format(header +
                            "{0}",
                                msg1, msg2));
                        break;
                    }
            }

        }
    }
}

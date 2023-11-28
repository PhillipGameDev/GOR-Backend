using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Email
{
    [DataContract]
    public class MailMessage
    {
        [DataMember]
        public string Subject;
        [DataMember]
        public string Message;
        [DataMember]
        public int SenderId;
        [DataMember]
        public string SenderName;

        public string Date;

        public MailMessage()
        {
        }
    }
}
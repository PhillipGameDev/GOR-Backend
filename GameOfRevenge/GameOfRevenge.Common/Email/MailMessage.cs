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

        public MailMessage()
        {
        }
    }
}
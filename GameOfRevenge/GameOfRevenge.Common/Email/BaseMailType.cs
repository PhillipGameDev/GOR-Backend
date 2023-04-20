using System;

namespace GameOfRevenge.Common.Email
{
    public class BaseMailType<T>
    {
        public int MailId { get; set; }
//        public int PlayerId { get; set; }
        public MailType MailType { get; set; }
        public bool Read { get; set; }
        public DateTime Date { get; set; }
        public T Content { get; set; }
    }
}

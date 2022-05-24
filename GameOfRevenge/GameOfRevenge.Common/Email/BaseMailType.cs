namespace GameOfRevenge.Common.Email
{
    public class BaseMailType<T>
    {
        public int PrimaryId { get; set; }
        public int PlayerId { get; set; }
        public MailType MailType { get; set; }
        public bool Read { get; set; }
        public T Content { get; set; }
    }
}

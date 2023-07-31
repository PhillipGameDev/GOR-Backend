namespace GameOfRevenge.WebAdmin.Models
{
    public class InputRestoreModel
    {
        public int PlayerId { get; set; }
        public long BackupId { get; set; }
        public string BackupDate { get; set; }

        public BackupData Data { get; set; }

        public InputRestoreModel()
        {
        }
    }

    public class BackupData
    {
        public long BackupId { get; set; }
        public string BackupDate { get; set; }
        public string Description { get; set; }
    }
}

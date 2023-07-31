using System;
using System.Data;
using System.Runtime.Serialization;
using GameOfRevenge.Common.Models.Table;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class PlayerBackupTable : BaseTable
    {
        [DataMember]
        public long BackupId { get; set; }
        [DataMember]
        public DateTime BackupDate { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Data { get; set; }

        public override void LoadFromDataReader(IDataReader reader)
        {
            int index = 0;
            BackupId = reader.GetValue(index) == DBNull.Value ? 0 : reader.GetInt64(index); index++;
            BackupDate = reader.GetValue(index) == DBNull.Value ? new DateTime() : reader.GetDateTime(index); index++;
            Description = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index); index++;
            if (index < reader.FieldCount)
            {
                Data = reader.GetValue(index) == DBNull.Value ? string.Empty : reader.GetString(index);
            }
        }
    }
}

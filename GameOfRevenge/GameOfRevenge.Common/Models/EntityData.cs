using System.Runtime.Serialization;

namespace GameOfRevenge.Common.Models
{
    [DataContract]
    public class EntityData
    {
        [DataMember(EmitDefaultValue = false)]
        public EntityType EntityType { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Health { get; set; }
        [DataMember]
        public int Attack { get; set; }
        [DataMember]
        public int Defense { get; set; }

        public EntityData()
        {
        }

        public EntityData(EntityType type, string name, int health, int attack, int defense)
        {
            EntityType = type;
            Name = name;
            Health = health;
            Attack = attack;
            Defense = defense;
        }
    }
}

namespace Server.Items
{
    public class EssencePassion : Item, ICommodity
    {
        [Constructable]
        public EssencePassion()
            : this(1)
        {
        }

        [Constructable]
        public EssencePassion(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1161;
        }

        public EssencePassion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113326;// essence of passion
        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}

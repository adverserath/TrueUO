namespace Server.Items
{
    public class EssenceDiligence : Item, ICommodity
    {
        [Constructable]
        public EssenceDiligence()
            : this(1)
        {
        }

        [Constructable]
        public EssenceDiligence(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1166;
        }

        public EssenceDiligence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113338;// essence of diligence
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

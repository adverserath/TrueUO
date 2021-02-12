namespace Server.Items
{
    public class CrimsonSwordBelt : SwordBelt
    {
        public override int LabelNumber => 1159212;  // crimson sword belt
        public override bool IsArtifact => true;

        [Constructable]
        public CrimsonSwordBelt()
        {
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
        }

        public CrimsonSwordBelt(Serial serial)
            : base(serial)
        {
        }

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

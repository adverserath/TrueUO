using Server.Gumps;

namespace Server.Items
{
    public class Note : Item
    {
        private string m_String;
        public string NoteString { get => m_String; set => m_String = value; }

        private int m_Number;
        public int Number { get => m_Number; set => m_Number = value; }

        [Constructable]
        public Note() : base(5357)
        {
        }

        [Constructable]
        public Note(string content) : base(5357)
        {
            m_String = content;
        }

        [Constructable]
        public Note(int number) : base(5357)
        {
            m_Number = number;
        }

        public Note(object content) : base(0x1234)
        {
            if (content is int i)
                m_Number = i;
            else if (content is string s)
                m_String = s;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 3))
            {
                m.CloseGump(typeof(InternalGump));
                m.SendGump(new InternalGump(this));
            }
        }

        private class InternalGump : Gump
        {
            public InternalGump(Note note) : base(50, 50)
            {
                AddImage(0, 0, 9380);
                AddImage(114, 0, 9381);
                AddImage(171, 0, 9382);
                AddImage(0, 140, 9386);
                AddImage(114, 140, 9387);
                AddImage(171, 140, 9388);

                if (note.NoteString != null)
                    AddHtml(38, 55, 200, 178, "<basefont color=#black>" + note.NoteString, false, true);
                else if (note.Number > 0)
                    AddHtmlLocalized(38, 55, 200, 178, note.Number, 1, false, true);
            }
        }

        public Note(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_String);
            writer.Write(m_Number);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_String = reader.ReadString();
            m_Number = reader.ReadInt();
        }
    }
}

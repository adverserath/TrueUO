using Server.Commands;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class GipsyGemologist : Mobile
    {
        [Constructable]
        public GipsyGemologist()
        {
            Name = "Zalia";
            Title = "The Gypsy Gemologist";
            Female = true;
            Race = Race.Human;
            Blessed = true;

            Hue = Utility.RandomSkinHue();

            AddItem(new Backpack());
            AddItem(new Shoes(0x737));
            AddItem(new Skirt(0x1BB));
            AddItem(new FancyShirt(0x535));

            Utility.AssignRandomHair(this);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!m.HasGump(typeof(ZaliaQuestGump)))
                {
                    m.SendGump(new ZaliaQuestGump());
                }
            }
            else
            {
                m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile m = from as PlayerMobile;

            if (m != null)
            {
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    if (dropped is AquaGem)
                    {
                        dropped.Delete();
                        from.AddToBackpack(new AquaPendant());

                        if (!m.HasGump(typeof(ZaliaQuestCompleteGump)))
                        {
                            m.SendGump(new ZaliaQuestCompleteGump());
                        }
                    }
                    else
                    {
                        PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in this.
                    }
                }
                else
                {
                    m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            return false;
        }

        public GipsyGemologist(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}

namespace Server.Gumps
{
    public class ZaliaQuestGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ZaliaQuest", AccessLevel.GameMaster, ZaliaQuestGump_OnCommand);
        }

        private static void ZaliaQuestGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ZaliaQuestGump());
        }

        public ZaliaQuestGump() : base(50, 50)
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154311, 0x7FFF, false, true); // Hello zere my darling - looking for something shiny? Zalia has just vhat you are looking for!

            AddHtmlLocalized(145, 300, 250, 24, 1154312, 0x7FFF, false, false); // I'm looking for a special pendant...            
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154313, 0x7FFF, false, true); // *Reads the note from Cousteau*  Oh another one of you zhen eh?  Zha Aqua pendant!  Might as well ask for zha crown jewels! I will craft zhis jewel for you if you acquire zha correct gemstone!

            AddHtmlLocalized(145, 300, 250, 24, 1154314, 0x7FFF, false, false); // Where do I find such a gemstone?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 140, 300, 150, 1154315, 0x7FFF, false, true); // Zha Aqua Gem! And I vhant a loaf of bread filled with gold!  *laughs* Oh, you vhere serious?  Well zhen ye must wrestle one avay from zhe Djinn.

            AddHtmlLocalized(145, 300, 250, 24, 1154316, 0x7FFF, false, false); // You want me to wrestle a Djinn!?!
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(4);
            AddHtmlLocalized(107, 140, 300, 150, 1154317, 0x7FFF, false, true); // Zhey are usually around the winding sandy paths around zhe camp here...oddly zhey are fond of zhe water...*shrugs*

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel
                        break;
                    }
            }
        }
    }

    public class ZaliaQuestCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ZaliaQuestComplete", AccessLevel.GameMaster, ZaliaQuestCompleteGump_OnCommand);
        }

        private static void ZaliaQuestCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ZaliaQuestCompleteGump());
        }

        public ZaliaQuestCompleteGump() : base(50, 50)
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154318, 0x7FFF, false, true); // Ahah! Yes, yes, zhat is indeed zhe gem! *does some quick tinkering*  Here is your pendant as you vish...

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel
                        break;
                    }
            }
        }
    }
}

using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;
using System.Linq;
using Server.Engines.Craft;

namespace Server.Items
{
    public class DragonCannon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DragonCannonDeed();

        [Constructable]
        public DragonCannon()
            : this(DirectionType.South)
        {
        }

        [Constructable]
        public DragonCannon(DirectionType direction)
        {
            switch (direction)
            {
                case DirectionType.North:
                    AddComponent(new AddonComponent(0x44F4), 0, 0, 0);
                    AddComponent(new AddonComponent(0x44F3), 0, 1, 0);
                    AddComponent(new AddonComponent(0x44F5), 0, -1, 0);
                    break;
                case DirectionType.West:
                    AddComponent(new AddonComponent(0x424A), 0, 0, 0);
                    AddComponent(new AddonComponent(0x4223), 1, 0, 0);
                    AddComponent(new AddonComponent(0x418F), -1, 0, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new AddonComponent(0x4221), 0, 0, 0);
                    AddComponent(new AddonComponent(0x4222), 0, 1, 0);
                    AddComponent(new AddonComponent(0x4220), 0, -1, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new AddonComponent(0x44F7), 0, 0, 0);
                    AddComponent(new AddonComponent(0x44F6), 1, 0, 0);
                    AddComponent(new AddonComponent(0x44F8), -1, 0, 0);
                    break;
            }
        }

        public DragonCannon(Serial serial)
            : base(serial)
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

    public class DragonCannonDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1158926;  // Decorative Dragon Cannon
        public override BaseAddon Addon => new DragonCannon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public DragonCannonDeed()
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.North, 1075389); // North
            list.Add((int)DirectionType.West, 1075390); // West
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076783)); // Please select your shadow altar position
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public DragonCannonDeed(Serial serial)
            : base(serial)
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

    [Flipable(0xA2CA, 0xA2CB)]
    public class ShoulderParrot : BaseOuterTorso
    {
        private DateTime _NextFly;
        private DateTime _FlyEnd;
        private Timer _Timer;
        private Mobile _LastShoulder;

        private string _MasterName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string MasterName { get => _MasterName; set { _MasterName = value; InvalidateProperties(); } }

        [Constructable]
        public ShoulderParrot()
            : base(0xA2CA)
        {
            LootType = LootType.Blessed;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_MasterName != null)
            {
                list.Add(1158958, string.Format("{0}{1}", _MasterName, _MasterName.ToLower().EndsWith("s") || _MasterName.ToLower().EndsWith("z") ? "'" : "'s"));
            }
            else
            {
                list.Add(1158928); // Shoulder Parrot
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.FindItemOnLayer(Layer.OuterTorso) == this)
            {
                if (_NextFly > DateTime.UtcNow)
                {
                    m.SendLocalizedMessage(1158956); // Your parrot is too tired to fly right now.
                }
                else
                {
                    _Timer = Timer.DelayCall(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500), FlyOnTick);
                    _Timer.Start();

                    Movable = false;
                    _LastShoulder = m;
                    MoveToWorld(new Point3D(m.X, m.Y, m.Z + 15), m.Map);
                    ItemID = 0xA2CC;

                    _FlyEnd = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(3, 5));
                }
            }
            else
            {
                m.SendLocalizedMessage(1158957); // Your parrot can't fly here.
            }
        }

        private void FlyOnTick()
        {
            if (_FlyEnd < DateTime.UtcNow && _LastShoulder != null)
            {
                Movable = true;
                ItemID = 0xA2CA;

                if (_LastShoulder.FindItemOnLayer(Layer.OuterTorso) != null)
                {
                    _LastShoulder.Backpack.DropItem(this);
                }
                else
                {
                    _LastShoulder.AddItem(this);
                }

                _LastShoulder = null;
                _Timer.Stop();
                _NextFly = DateTime.UtcNow + TimeSpan.FromMinutes(2);
            }
        }

        public ShoulderParrot(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_MasterName);
            writer.Write(_LastShoulder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _MasterName = reader.ReadString();
            Mobile m = reader.ReadMobile();

            if (m != null)
            {
                ItemID = 0xA2CA;

                Timer.DelayCall(() =>
                {
                    if (m.FindItemOnLayer(Layer.OuterTorso) != null)
                    {
                        m.Backpack.DropItem(this);
                    }
                    else
                    {
                        m.AddItem(this);
                    }
                });
            }
        }
    }

    [Flipable(0xA2C8, 0xA2C9)]
    public class PirateWallMap : Item
    {
        public override int LabelNumber => 1158938;  // Pirate Wall Map

        [Constructable]
        public PirateWallMap()
            : base(0xA2C8)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                Gump gump = new Gump(50, 50);
                gump.AddImage(0, 0, 0x9CE9);

                m.SendGump(gump);
            }
        }

        public PirateWallMap(Serial serial) : base(serial)
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

    [Flipable(0xA2C6, 0xA2C7)]
    public class MysteriousStatue : Item
    {
        public override int LabelNumber => 1158935; // Mysterious Statue

        [Constructable]
        public MysteriousStatue()
            : base(0xA2C6)
        {
            Weight = 5.0;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                if (m.Skills[SkillName.Carpentry].Value >= 100)
                {
                    Gump g = new Gump(100, 100);
                    g.AddBackground(0, 0, 454, 400, 0x24A4);
                    g.AddItem(35, 120, 0xA2C6);
                    g.AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1158935", 0x3442, false, false); // Mysterious Statue
                    g.AddHtmlLocalized(177, 77, 250, 36, 1114513, "#1158936", 0x3442, false, false); // Purchased from a Pirate Merchant
                    g.AddHtmlLocalized(177, 122, 250, 228, 1158937, 0xC63, true, true);
                    /*This mysterious statue towers above you. Even as skilled a mason as you are, the craftsmanship is uncanny, and unlike anything you have encountered before.
                    *The stone appears to be smooth and special attention was taken to sculpt the statue as a perfect likeness. According to the pirate you purchased the statue
                    *from, it was recovered somewhere at sea. The amount of marine growth seems to reinforce this claim, yet you cannot discern how long it may have been
                    * submerged and are thus unsure of its age.Whatever its origins, one thing is clear - the figure is one you hope you do not encounter anytime soon...
                    */

                    m.SendGump(g);
                    m.SendSound(m.Female ? 0x30B : 0x41A);

                    m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157722, "Carpentry", m.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*
                }
                else
                {
                    m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157693, "Carpentry", m.NetState); // *You lack the required ~1_SKILL~ skill to make anything of it.*
                    m.SendSound(m.Female ? 0x31F : 0x42F);
                }
            }
        }

        public MysteriousStatue(Serial serial) : base(serial)
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

    [Flipable(0x4C26, 0x4C27)]
    public class DecorativeWoodCarving : Item
    {
        public string _ShipName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName { get => _ShipName; set { _ShipName = value; InvalidateProperties(); } }

        [Constructable]
        public DecorativeWoodCarving()
            : base(0x4C26)
        {
            Hue = 2968;
        }

        public void AssignRandomName()
        {
            System.Collections.Generic.List<string> list = BaseBoat.Boats.Where(b => !string.IsNullOrEmpty(b.ShipName)).Select(x => x.ShipName).ToList();

            _ShipName = list.Count > 0 ? list[Utility.Random(list.Count)] : _ShipNames[Utility.Random(_ShipNames.Length)];

            InvalidateProperties();
            ColUtility.Free(list);
        }

        private static readonly string[] _ShipNames =
        {
            "Adventure Galley",
            "Queen Anne's Revenge",
            "Fancy",
            "Whydah",
            "Royal Fortune",
            "The Black Pearl",
            "Satisfaction",
            "The Golden Fleece",
            "Bachelor's Delight",
            "The Revenge",
            "The Flying Dragon",
            "The Gabriel",
            "Privateer's Death",
            "Kiss of Death",
            "Devil's Doom",
            "Monkeebutt",
            "Mourning Star",
            "Cursed Sea-Dog",
            "The Howling Lusty Wench",
            "Scourage of the Seven Seas",
            "Neptune's Plague",
            "Sea's Hellish Plague",
            "The Salty Bastard"
        };

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (string.IsNullOrEmpty(_ShipName))
            {
                list.Add(1158943); // Wood Carving of [Ship's Name]
            }
            else
            {
                list.Add(1158921, _ShipName); // Wood Carving of ~1_name~
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (string.IsNullOrEmpty(_ShipName))
            {
                list.Add(1158953); // Named with a random famous ship, or if yer lucky - named after you!
            }
        }

        public DecorativeWoodCarving(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_ShipName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _ShipName = reader.ReadString();
        }
    }

    public class QuartermasterRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158951;  // Quartermaster

        [Constructable]
        public QuartermasterRewardDeed()
        {
        }

        public QuartermasterRewardDeed(Serial serial)
            : base(serial)
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

    public class SailingMasterRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158950;  // Sailing Master

        [Constructable]
        public SailingMasterRewardDeed()
        {
        }

        public SailingMasterRewardDeed(Serial serial)
            : base(serial)
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

    public class BotswainRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158949;  // Botswain

        [Constructable]
        public BotswainRewardDeed()
        {
        }

        public BotswainRewardDeed(Serial serial)
            : base(serial)
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

    public class PowderMonkeyRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158948;  // Powder Monkey

        [Constructable]
        public PowderMonkeyRewardDeed()
        {
        }

        public PowderMonkeyRewardDeed(Serial serial)
            : base(serial)
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

    public class SpikedWhipOfPlundering : SpikedWhip
    {
        public override int LabelNumber => 1158925;  // Spiked Whip of Plundering

        [Constructable]
        public SpikedWhipOfPlundering()
        {
            ExtendedWeaponAttributes.HitExplosion = 15;
            WeaponAttributes.HitLeechMana = 81;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 70;
        }

        public SpikedWhipOfPlundering(Serial serial)
            : base(serial)
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

    public class BladedWhipOfPlundering : BladedWhip
    {
        public override int LabelNumber => 1158924;  // Bladed Whip of Plundering

        [Constructable]
        public BladedWhipOfPlundering()
        {
            ExtendedWeaponAttributes.HitExplosion = 15;
            WeaponAttributes.HitLeechMana = 81;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 70;
        }

        public BladedWhipOfPlundering(Serial serial)
            : base(serial)
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

    public class BarbedWhipOfPlundering : BarbedWhip
    {
        public override int LabelNumber => 1158923;  // Barbed Whip of Plundering

        [Constructable]
        public BarbedWhipOfPlundering()
        {
            ExtendedWeaponAttributes.HitExplosion = 15;
            WeaponAttributes.HitLeechMana = 81;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 70;
        }

        public BarbedWhipOfPlundering(Serial serial)
            : base(serial)
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

    public class PeculiarCoconut : Item
    {
        public override int LabelNumber => IsPalmTree ? 1159580 : 1159572;  // palm tree - Peculiar Coconut

        public bool IsPalmTree => ItemID != 0xA73E;

        [Constructable]
        public PeculiarCoconut()
            : base(0xA73E)
        {
        }

        public PeculiarCoconut(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsPalmTree)
            {
                if (IsSecure || IsLockedDown)
                {
                    ItemID = Utility.RandomList(0xA63F, 0xA640, 0xA641, 0xA642, 0xA643, 0xA644);
                    Weight = 5;
                }
                else
                {
                    from.SendLocalizedMessage(1112573); // This must be locked down or secured in order to use it.
                }
            }
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

    public class PirateChestTreasure : Item
    {
        public Item Chest { get; }

        [Constructable]
        public PirateChestTreasure(int id, Item chest)
            : base(id)
        {
            Chest = chest;
            Movable = false;
        }

        public PirateChestTreasure(Serial serial)
            : base(serial)
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

            if (Chest == null)
                Delete();
        }
    }

    [Furniture]
    public class PirateChest : BaseContainer, IFlipable
    {
        public override int LabelNumber => 1015097; // Chest    

        public override int DefaultGumpID => 0x49;

        public Item Treasure { get; set; }

        [Constructable]
        public PirateChest()
            : base(0xA639)
        {
            Weight = 2.0;
        }

        public PirateChest(Serial serial)
            : base(serial)
        {
        }

        public void OnFlip(Mobile m)
        {
            switch (ItemID)
            {
                case 0xA639:
                    ItemID = 0xA63A;
                    break;
                case 0xA63A:
                    ItemID = 0xA639;
                    break;
            }
        }

        public override void Open(Mobile from)
        {
            switch (ItemID)
            {
                default:
                case 0xA639: ItemID = 0xA63B; AddTreasure(0xA63D); break;
                case 0xA63A: ItemID = 0xA63C; AddTreasure(0xA63E); break;
                case 0xA63B: ItemID = 0xA639; RemoveTreasure(); break;
                case 0xA63C: ItemID = 0xA63A; RemoveTreasure(); break;
            }

            DisplayTo(from);
        }

        public void AddTreasure(int id)
        {
            if (IsLockedDown || IsSecure)
            {
                Treasure = new PirateChestTreasure(id, this);
                Treasure.MoveToWorld(new Point3D(X, Y, Z + 1), Map);
            }
        }

        public void RemoveTreasure()
        {
            Treasure?.Delete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (Treasure != null)
            {
                if (IsLockedDown || IsSecure)
                {
                    Treasure.Location = new Point3D(X, Y, Z + 1);
                }
                else
                {
                    RemoveTreasure();
                }
            }                
        }

        public override void OnLockDownChange()
        {
            if (IsLockedDown)
            {
                RemoveTreasure();
            }
        }

        public override void OnSecureChange()
        {
            if (IsSecure)
            {
                RemoveTreasure();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            RemoveTreasure();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Treasure);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Treasure = reader.ReadItem();
        }
    }

    public class Orchid1 : Item
    {
        public override int LabelNumber => 1159575;  // orchid

        [Constructable]
        public Orchid1()
            : base(0xA648)
        {
            if (0.15 >= Utility.RandomDouble())
            {
                Hue = Utility.RandomList(1100, 1153, 1173, 1918, 1289, 2735, 2736, 2753);
            }
            else
            {
                Hue = Utility.RandomList(0, 2, 12, 17, 38, 43, 52, 67);
            }           
        }

        public Orchid1(Serial serial)
            : base(serial)
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

    public class Orchid2 : Item
    {
        public override int LabelNumber => 1159575;  // orchid

        [Constructable]
        public Orchid2()
            : base(0xA647)
        {
            if (0.15 >= Utility.RandomDouble())
            {
                Hue = Utility.RandomList(1100, 1153, 1173, 1918, 1289, 2735, 2736, 2753);
            }
            else
            {
                Hue = Utility.RandomList(0, 2, 12, 17, 38, 43, 52, 67);
            }
        }

        public Orchid2(Serial serial)
            : base(serial)
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

    public class Orchid3 : Item
    {
        public override int LabelNumber => 1159575;  // orchid

        [Constructable]
        public Orchid3()
            : base(0xA646)
        {
            if (0.15 >= Utility.RandomDouble())
            {
                Hue = Utility.RandomList(1100, 1153, 1173, 1918, 1289, 2735, 2736, 2753);
            }
            else
            {
                Hue = Utility.RandomList(0, 2, 12, 17, 38, 43, 52, 67);
            }
        }

        public Orchid3(Serial serial)
            : base(serial)
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

    public class LandlubberRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1159576;  // Landlubber

        [Constructable]
        public LandlubberRewardDeed()
        {
        }

        public LandlubberRewardDeed(Serial serial)
            : base(serial)
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

    public class SeaWaspRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1159577;  // Sea Wasp

        [Constructable]
        public SeaWaspRewardDeed()
        {
        }

        public SeaWaspRewardDeed(Serial serial)
            : base(serial)
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

    public class FirstMateRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1159578;  // First Mate

        [Constructable]
        public FirstMateRewardDeed()
        {
        }

        public FirstMateRewardDeed(Serial serial)
            : base(serial)
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

    public class PirateLordRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1159579;  // Pirate Lord

        [Constructable]
        public PirateLordRewardDeed()
        {
        }

        public PirateLordRewardDeed(Serial serial)
            : base(serial)
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

    public class PirateShieldRecipeScroll : RecipeScroll
    {
        [Constructable]
        public PirateShieldRecipeScroll()
            : base(172)
        {
        }

        public PirateShieldRecipeScroll(Serial serial)
            : base(serial)
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

    [Flipable(0xA649, 0xA64A)]
    public class HooksShield : BaseShield, IRepairable
    {
        public override int LabelNumber => 1159584; // Hook's Shield
        public override bool IsArtifact => true;
        public CraftSystem RepairSystem => DefCarpentry.CraftSystem;

        [Constructable]
        public HooksShield()
            : base(0xA649)
        {
            Weight = 8.0;
            
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 15;
            Attributes.SpellDamage = 10;
            Attributes.CastSpeed = 1;
        }

        public HooksShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int StrReq => 20;

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }
    }

    public class HooksTreasureMap : Item
    {
        public override int LabelNumber => 1159641; // Hook's Treasure Map

        [Constructable]
        public HooksTreasureMap()
            : base(0x14ED)
        {
            Hue = 2721;
        }

        public HooksTreasureMap(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(HooksTreasureMapGump)))
            {
                from.SendGump(new HooksTreasureMapGump());
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1157722, "its origin"); // *Your proficiency in ~1_SKILL~ reveals more about the item*
            }
        }

        private class HooksTreasureMapGump : Gump
        {
            public HooksTreasureMapGump()
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 480, 320, 0x6DB);
                AddSpriteImage(24, 24, 0x474, 60, 60, 108, 108);
                AddImage(15, 15, 0xA9F);
                AddImageTiledButton(22, 22, 0x176F, 0x176F, 0x0, GumpButtonType.Page, 0, 0x14ED, 0xAA1, 33, 44);
                AddHtml(150, 15, 320, 22, "<BASEFONT COLOR=#D5D52A><DIV ALIGN=CENTER>Hook's Treasure Map</DIV></BASEFONT>", false, false);
                AddHtml(150, 46, 320, 44, "<BASEFONT COLOR=#AABFD4><DIV ALIGN=CENTER>Purchased from the Black Market</DIV></BASEFONT>", false, false);
                AddHtml(150, 99, 320, 98, "<BASEFONT COLOR=#DFDFDF>The map likely leads to great treasure, however understanding it is beyond your comprehension.</BASEFONT>", false, false);
                AddHtml(150, 197, 320, 98, "<BASEFONT COLOR=#DFDFDF>Deciphering it will require one with the reputation as a famed Artifact Hunter.</BASEFONT>", false, false);
            }
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

using L2Monitor.Game.Models.Items;
using L2Monitor.Game.Packets.Incomming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Game.Models
{
    public class CharacterInSelectScreen
    {

        private readonly int[] PAPERDOLL_ORDER = new int[]
        {
            (int)PaperDoll.PAPERDOLL_UNDER,
            (int)PaperDoll.PAPERDOLL_REAR,
            (int)PaperDoll.PAPERDOLL_LEAR,
            (int)PaperDoll.PAPERDOLL_NECK,
            (int)PaperDoll.PAPERDOLL_RFINGER,
            (int)PaperDoll.PAPERDOLL_LFINGER,
            (int)PaperDoll.PAPERDOLL_HEAD,
            (int)PaperDoll.PAPERDOLL_RHAND,
            (int)PaperDoll.PAPERDOLL_LHAND,
            (int)PaperDoll.PAPERDOLL_GLOVES,
            (int)PaperDoll.PAPERDOLL_CHEST,
            (int)PaperDoll.PAPERDOLL_LEGS,
            (int)PaperDoll.PAPERDOLL_FEET,
            (int)PaperDoll.PAPERDOLL_CLOAK,
            (int)PaperDoll.PAPERDOLL_UNKNOWN,
            (int)PaperDoll.PAPERDOLL_HAIR,
            (int)PaperDoll.PAPERDOLL_HAIR2,
            (int)PaperDoll.PAPERDOLL_RBRACELET,
            (int)PaperDoll.PAPERDOLL_LBRACELET,
            (int)PaperDoll.PAPERDOLL_AGATHION1, // 152
		    (int)PaperDoll.PAPERDOLL_AGATHION2, // 152
		    (int)PaperDoll.PAPERDOLL_AGATHION3, // 152
		    (int)PaperDoll.PAPERDOLL_AGATHION4, // 152
		    (int)PaperDoll.PAPERDOLL_AGATHION5, // 152
		    (int)PaperDoll.PAPERDOLL_DECO1,
            (int)PaperDoll.PAPERDOLL_DECO2,
            (int)PaperDoll.PAPERDOLL_DECO3,
            (int)PaperDoll.PAPERDOLL_DECO4,
            (int)PaperDoll.PAPERDOLL_DECO5,
            (int)PaperDoll.PAPERDOLL_DECO6,
            (int)PaperDoll.PAPERDOLL_BELT,
            (int)PaperDoll.PAPERDOLL_BROOCH,
            (int)PaperDoll.PAPERDOLL_BROOCH_JEWEL1,
            (int)PaperDoll.PAPERDOLL_BROOCH_JEWEL2,
            (int)PaperDoll.PAPERDOLL_BROOCH_JEWEL3,
            (int)PaperDoll.PAPERDOLL_BROOCH_JEWEL4,
            (int)PaperDoll.PAPERDOLL_BROOCH_JEWEL5,
            (int)PaperDoll.PAPERDOLL_BROOCH_JEWEL6,
            (int)PaperDoll.PAPERDOLL_ARTIFACT_BOOK, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT1, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT2, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT3, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT4, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT5, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT6, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT7, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT8, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT9, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT10, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT11, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT12, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT13, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT14, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT15, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT16, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT17, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT18, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT19, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT20, // 152
		    (int)PaperDoll.PAPERDOLL_ARTIFACT21 // 152
        };

        private readonly int[] PAPERDOLL_ORDER_VISUAL_ID = new int[]
        {
            (int)PaperDoll.PAPERDOLL_RHAND,
            (int)PaperDoll.PAPERDOLL_LHAND,
            (int)PaperDoll.PAPERDOLL_GLOVES,
            (int)PaperDoll.PAPERDOLL_CHEST,
            (int)PaperDoll.PAPERDOLL_LEGS,
            (int)PaperDoll.PAPERDOLL_FEET,
            (int)PaperDoll.PAPERDOLL_UNKNOWN,
            (int)PaperDoll.PAPERDOLL_HAIR,
            (int)PaperDoll.PAPERDOLL_HAIR2,
        };

        private readonly int[] ARMOR_ENCHANT_PARTS = new int[]
        {
            (int)PaperDoll.PAPERDOLL_CHEST, // Upper Body enchant level
            (int)PaperDoll.PAPERDOLL_LEGS, // Lower Body enchant level
            (int)PaperDoll.PAPERDOLL_HEAD, // Headgear enchant level
            (int)PaperDoll.PAPERDOLL_GLOVES, // Gloves enchant level
            (int)PaperDoll.PAPERDOLL_FEET, // Boots enchant level
        };

        public string CharName { get; set; }
        public int CharId { get; set; }
        public string LoginName { get; set; }
        public int SessionId { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Sex { get; set; }
        public int Race { get; set; }
        public int Class { get; set; }
        public int Unknown3 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public double CurrentHp { get; set; }
        public double CurrentMp { get; set; }
        public long CurrentXp { get; set; }
        public long CurrentSp { get; set; }
        public double CurrentXpPercent { get; set; }
        public int Level { get; set; }
        public int Reputation { get; private set; }
        public int PKKills { get; private set; }
        public int PVPKills { get; private set; }
        public int Unknown4 { get; private set; }
        public int Unknown5 { get; private set; }
        public int Unknown6 { get; private set; }
        public int Unknown7 { get; private set; }
        public int Unknown8 { get; private set; }
        public int Unknown9 { get; private set; }
        public int Unknown10 { get; private set; }
        public int Unknown11 { get; private set; }
        public int Unknown12 { get; private set; }

        public Dictionary<PaperDoll, int> SelectedInventory { get; set; }
        public Dictionary<PaperDoll, int> VisualInventory { get; set; }
        public Dictionary<PaperDoll, short> ArmorEnchantLevels { get; set; }
        public int HairStyle { get; private set; }
        public int HairColor { get; private set; }
        public int FaceStyle { get; private set; }
        public double MaxHP { get; private set; }
        public double MaxMP { get; private set; }
        public int DeleteTimer { get; private set; }
        public int ClassId { get; private set; }
        public int IsActive { get; private set; }
        public byte WeaponEnchantEffect { get; private set; }
        public int WeaponAugument1 { get; private set; }
        public int WeaponAugument2 { get; private set; }
        /// <summary>
        /// Currently on retail when you are on character select you don't see your transformation.
        /// </summary>
        public int TransformationId { get; private set; }
        public int PetId { get; private set; }
        public int PetLevel { get; private set; }
        public int PetFood { get; private set; }
        public int PetFoodLevel { get; private set; }
        public double PetHP { get; private set; }
        public double PetMP { get; private set; }
        public int VitalityPoints { get; private set; }
        public int VitalityPercent { get; private set; }
        public int RemainingVitalityUses { get; private set; }
        public int CharActive { get; private set; }
        public bool IsNoble { get; private set; }
        public bool IsHero { get; private set; }
        public bool HairAccessoryEnabled { get; private set; }
        public int BanTimeLeft { get; private set; }
        public int LastAccess { get; private set; }

        public CharacterInSelectScreen(CharSelectInfo stream)
        {
            SelectedInventory = new Dictionary<PaperDoll, int>();
            VisualInventory = new Dictionary<PaperDoll, int>();
            ArmorEnchantLevels = new Dictionary<PaperDoll, short>();


            CharName = stream.readString();
            CharId = stream.readInt();
            LoginName = stream.readString();
            SessionId = stream.readInt();
            Unknown1 = stream.readInt();
            Unknown2 = stream.readInt();
            Sex = stream.readInt();
            Race = stream.readInt();
            Class = stream.readInt();
            Unknown3 = stream.readInt();
            X = stream.readInt();
            Y = stream.readInt();
            Z = stream.readInt();
            CurrentHp = stream.readDouble();
            CurrentMp = stream.readDouble();
            CurrentSp = stream.readLong();
            CurrentXp = stream.readLong();
            CurrentXpPercent = stream.readDouble() * 100;
            Level = stream.readInt();
            Reputation = stream.readInt();
            PKKills = stream.readInt();
            PVPKills = stream.readInt();

            Unknown4 = stream.readInt();
            Unknown5 = stream.readInt();
            Unknown6 = stream.readInt();
            Unknown7 = stream.readInt();
            Unknown8 = stream.readInt();
            Unknown9 = stream.readInt();
            Unknown10 = stream.readInt();

            Unknown11 = stream.readInt(); //Ertheia
            Unknown12 = stream.readInt(); //Ertheia

            foreach (var slot in PAPERDOLL_ORDER)
            {
                var key = (PaperDoll)slot;
                var value = stream.readInt();
                SelectedInventory.Add(key, value);
            }
            foreach (var slot in PAPERDOLL_ORDER_VISUAL_ID)
            {
                var value = stream.readInt();
                VisualInventory.Add((PaperDoll)slot, value);
            }

            foreach (var slot in ARMOR_ENCHANT_PARTS)
            {
                var value = stream.readInt16();
                ArmorEnchantLevels.Add((PaperDoll)slot, value);
            }
            HairStyle = stream.readInt();
            HairColor = stream.readInt();
            FaceStyle = stream.readInt();

            MaxHP = stream.readDouble();
            MaxMP = stream.readDouble();
            DeleteTimer = stream.readInt();
            ClassId = stream.readInt();
            IsActive = stream.readInt();
            WeaponEnchantEffect = stream.readByte();
            WeaponAugument1 = stream.readInt();
            WeaponAugument2 = stream.readInt();
            TransformationId = stream.readInt();
            PetId = stream.readInt();
            PetLevel = stream.readInt();
            PetFood = stream.readInt();
            PetFoodLevel = stream.readInt();
            PetHP = stream.readDouble();
            PetMP = stream.readDouble();
            VitalityPoints = stream.readInt();
            VitalityPercent = stream.readInt();
            RemainingVitalityUses = stream.readInt();
            CharActive = stream.readInt();
            IsNoble = stream.readBool();
            IsHero = stream.readBool();
            HairAccessoryEnabled = stream.readBool();
            BanTimeLeft = stream.readInt();
            LastAccess = stream.readInt();
        }
    }
}

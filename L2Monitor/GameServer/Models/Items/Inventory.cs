using System;
using System.Collections.Generic;
using System.Text;

namespace L2Monitor.GameServer.Models.Items
{
	public class Inventory
	{
		// Common Items
		public const int ADENA_ID = 57;
		public const int ANCIENT_ADENA_ID = 5575;
		public const int BEAUTY_TICKET_ID = 36308;
		public const int AIR_STONE_ID = 39461;
		public const int TEMPEST_STONE_ID = 39592;
		public const int ELCYUM_CRYSTAL_ID = 36514;

		public const int PAPERDOLL_UNDER = 0;
		public const int PAPERDOLL_HEAD = 1;
		public const int PAPERDOLL_HAIR = 2;
		public const int PAPERDOLL_HAIR2 = 3;
		public const int PAPERDOLL_NECK = 4;
		public const int PAPERDOLL_RHAND = 5;
		public const int PAPERDOLL_CHEST = 6;
		public const int PAPERDOLL_LHAND = 7;
		public const int PAPERDOLL_REAR = 8;
		public const int PAPERDOLL_LEAR = 9;
		public const int PAPERDOLL_GLOVES = 10;
		public const int PAPERDOLL_LEGS = 11;
		public const int PAPERDOLL_FEET = 12;
		public const int PAPERDOLL_RFINGER = 13;
		public const int PAPERDOLL_LFINGER = 14;
		public const int PAPERDOLL_LBRACELET = 15;
		public const int PAPERDOLL_RBRACELET = 16;
		public const int PAPERDOLL_AGATHION1 = 17;
		public const int PAPERDOLL_AGATHION2 = 18;
		public const int PAPERDOLL_AGATHION3 = 19;
		public const int PAPERDOLL_AGATHION4 = 20;
		public const int PAPERDOLL_AGATHION5 = 21;
		public const int PAPERDOLL_DECO1 = 22;
		public const int PAPERDOLL_DECO2 = 23;
		public const int PAPERDOLL_DECO3 = 24;
		public const int PAPERDOLL_DECO4 = 25;
		public const int PAPERDOLL_DECO5 = 26;
		public const int PAPERDOLL_DECO6 = 27;
		public const int PAPERDOLL_CLOAK = 28;
		public const int PAPERDOLL_BELT = 29;
		public const int PAPERDOLL_BROOCH = 30;
		public const int PAPERDOLL_BROOCH_JEWEL1 = 31;
		public const int PAPERDOLL_BROOCH_JEWEL2 = 32;
		public const int PAPERDOLL_BROOCH_JEWEL3 = 33;
		public const int PAPERDOLL_BROOCH_JEWEL4 = 34;
		public const int PAPERDOLL_BROOCH_JEWEL5 = 35;
		public const int PAPERDOLL_BROOCH_JEWEL6 = 36;
		public const int PAPERDOLL_ARTIFACT_BOOK = 37;
		public const int PAPERDOLL_ARTIFACT1 = 38; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT2 = 39; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT3 = 40; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT4 = 41; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT5 = 42; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT6 = 43; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT7 = 44; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT8 = 45; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT9 = 46; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT10 = 47; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT11 = 48; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT12 = 49; // Artifact Balance
		public const int PAPERDOLL_ARTIFACT13 = 50; // Artifact Spirit
		public const int PAPERDOLL_ARTIFACT14 = 51; // Artifact Spirit
		public const int PAPERDOLL_ARTIFACT15 = 52; // Artifact Spirit
		public const int PAPERDOLL_ARTIFACT16 = 53; // Artifact Protection
		public const int PAPERDOLL_ARTIFACT17 = 54; // Artifact Protection
		public const int PAPERDOLL_ARTIFACT18 = 55; // Artifact Protection
		public const int PAPERDOLL_ARTIFACT19 = 56; // Artifact Support
		public const int PAPERDOLL_ARTIFACT20 = 57; // Artifact Support
		public const int PAPERDOLL_ARTIFACT21 = 58; // Artifact Support

		public static int PAPERDOLL_TOTALSLOTS = Enum.GetNames(typeof(PaperDoll)).Length;
		public static int BLA = Enum.GetNames(typeof(PaperDoll)).Length;

	}
}

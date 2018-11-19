using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
	public class BossTrophy : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(Type);
			dustType = 7;
			disableSmartCursor = true;
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Trophy");
 			AddMapEntry(new Color(120, 85, 60), name);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int item = 0;
			switch (frameX / 54)
			{
				case 0:
					item = mod.ItemType("DesertScourgeTrophy");
					break;
				case 1:
					item = mod.ItemType("PerforatorTrophy");
					break;
				case 2:
					item = mod.ItemType("SlimeGodTrophy");
					break;
				case 3:
					item = mod.ItemType("CryogenTrophy");
					break;
				case 4:
					item = mod.ItemType("PlaguebringerGoliathTrophy");
					break;
				case 5:
					item = mod.ItemType("LeviathanTrophy");
					break;
				case 6:
					item = mod.ItemType("ProvidenceTrophy");
					break;
				case 7:
					item = mod.ItemType("CalamitasTrophy");
					break;
				case 8:
					item = mod.ItemType("HiveMindTrophy");
					break;
				case 9:
					item = mod.ItemType("CrabulonTrophy");
					break;
				case 10:
					item = mod.ItemType("YharonTrophy");
					break;
				case 11:
					item = mod.ItemType("SignusTrophy");
					break;
				case 12:
					item = mod.ItemType("WeaverTrophy");
					break;
				case 13:
					item = mod.ItemType("CeaselessVoidTrophy");
					break;
				case 14:
					item = mod.ItemType("DevourerofGodsTrophy");
					break;
				case 15:
					item = mod.ItemType("CatastropheTrophy");
					break;
				case 16:
					item = mod.ItemType("CataclysmTrophy");
					break;
                case 17:
                    item = mod.ItemType("PolterghastTrophy");
                    break;
                case 18:
                    item = mod.ItemType("BumblebirbTrophy");
                    break;
                case 19:
                    item = mod.ItemType("AstrageldonTrophy");
                    break;
                case 20:
                    item = mod.ItemType("AstrumDeusTrophy");
                    break;
                case 21:
                    item = mod.ItemType("BrimstoneElementalTrophy");
                    break;
				case 22:
                    item = mod.ItemType("RavagerTrophy");
                    break;	
            }
			if (item > 0)
			{
				Item.NewItem(i * 16, j * 16, 48, 48, item);
			}
		}
	}
}
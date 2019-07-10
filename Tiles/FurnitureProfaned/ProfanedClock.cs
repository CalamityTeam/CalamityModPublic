using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureProfaned
{
	public class ProfanedClock : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
            animationFrameHeight = 90;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16
			};
            TileObjectData.newTile.Origin = new Point16(0, 4);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
            name.SetDefault("Profaned Clock");
            AddMapEntry(new Color(191, 142, 111), name);
            dustType = mod.DustType("Pixel");
			adjTiles = new int[] { TileID.GrandfatherClocks };
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 246, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, mod.DustType("ProfanedTileRock"), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RightClick(int x, int y)
		{
			{
				string text = "AM";
				//Get current weird time
				double time = Main.time;
				if (!Main.dayTime)
				{
					//if it's night add this number
					time += 54000.0;
				}
				//Divide by seconds in a day * 24
				time = time / 86400.0 * 24.0;
				//Dunno why we're taking 19.5. Something about hour formatting
				time = time - 7.5 - 12.0;
				//Format in readable time
				if (time < 0.0)
				{
					time += 24.0;
				}
				if (time >= 12.0)
				{
					text = "PM";
				}
				int intTime = (int)time;
				//Get the decimal points of time.
				double deltaTime = time - intTime;
				//multiply them by 60. Minutes, probably
				deltaTime = ((int)(deltaTime * 60.0));
				//This could easily be replaced by deltaTime.ToString()
				string text2 = string.Concat(deltaTime);
				if (deltaTime < 10.0)
				{
					//if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
					text2 = "0" + text2;
				}
				if (intTime > 12)
				{
					//This is for AM/PM time rather than 24hour time
					intTime -= 12;
				}
				if (intTime == 0)
				{
					//0AM = 12AM
					intTime = 12;
				}
				//Whack it all together to get a HH:MM format
				var newText = string.Concat("Time: ", intTime, ":", text2, " ", text);
				Main.NewText(newText, 255, 240, 20);
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer)
			{
				Main.clock = true;
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 48, 32, mod.ItemType("ProfanedClock"));
		}
	}
}

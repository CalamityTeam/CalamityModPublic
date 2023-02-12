using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace CalamityMod.Tiles.Crags.Tree
{
	public class SpineSapling : ModTile
	{
		public override void SetStaticDefaults() 
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.CommonSapling[Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<Tiles.Crags.BrimstoneSlag>() };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawFlipHorizontal = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.StyleMultiplier = 3;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
            name.SetDefault("Spine Sapling");
            AddMapEntry(new Color(38, 25, 27), name);
            DustType = DustID.Blood;
			AdjTiles = new int[] { TileID.Saplings };
		}

        public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = fail ? 1 : 3;
		}

		public override void RandomUpdate(int i, int j) 
		{
			if (WorldGen.genRand.Next(20) == 0)
            {
                bool isPlayerNear = WorldGen.PlayerLOS(i, j);

				if (isPlayerNear && Main.tile[i, j + 1].TileType != ModContent.TileType<SpineSapling>())
				{
					SpineTree.Spawn(i, j, 22, 28, true);
				}
			}
		}
	}
}
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Tiles.Astral
{
	public class AstralOre : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileValue[Type] = 700;
            Main.tileSpelunker[Type] = true;
            Main.tileShine2[Type] = true;

            minPick = 200;
			dustType = 173;
			drop = mod.ItemType("AstralOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Astral Ore");
 			AddMapEntry(new Color(255, 153, 255), name);
			mineResist = 5f;
			soundType = 21;
            
            TileID.Sets.Ore[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
        }
		
		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
            return CalamityWorld.downedStarGod;
		}
		
		public override bool CanExplode(int i, int j)
        {
            return CalamityWorld.downedStarGod;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override void RandomUpdate(int i, int j)
        {
            if (Main.rand.Next(7) <= 2)
            {
                Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, 156, Main.rand.NextFloat(-0.05f, 0.05f), Main.rand.NextFloat(-0.05f, -0.001f));
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.09f;
            g = 0.03f;
            b = 0.07f;

            float minStrength = 3.4f;
            float bonusStrength = 0.5f;
            float strength = minStrength + (float)Math.Sin(MathHelper.ToRadians((float)(Main.time / 6.0))) * bonusStrength;
            Lighting.AddLight(new Vector2(i * 16 + 8f, j * 16 + 8f), r * strength, g * strength, b * strength);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, mod.TileType("AstralDirt"), false, false, false, false, resetFrame);
            return false;
        }
    }
}
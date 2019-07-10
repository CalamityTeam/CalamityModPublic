using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class ChaoticOre : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileValue[Type] = 750;
			dustType = 105;
			drop = mod.ItemType("ChaoticOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Chaotic Ore");
 			AddMapEntry(new Color(255, 0, 0), name);
			mineResist = 4f;
			minPick = 210;
			soundType = 21;
		}

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer && j < Main.maxTilesY - 205)
            {
                if (Main.tile[i, j].liquid <= 0)
                {
                    Main.tile[i, j].liquid = 255;
                    Main.tile[i, j].lava(false);
                }
            }
            if (Main.gamePaused)
            {
                return;
            }
            if (closer && Main.rand.Next(300) == 0)
            {
                int tileLocationY = j + 1;
                if (Main.tile[i, tileLocationY] != null)
                {
                    if (!Main.tile[i, tileLocationY].active())
                    {
                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile((float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, 0.1f, mod.ProjectileType("LavaChunk"), 25, 2f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
        }

        public override bool CanExplode(int i, int j)
		{
			return NPC.downedGolemBoss;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.04f;
            g = 0.00f;
            b = 0.00f;
        }
    }
}

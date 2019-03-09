using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class Tenebris : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = 44;
			drop = mod.ItemType("Tenebris");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Tenebris");
 			AddMapEntry(new Color(0, 100, 100), name);
			mineResist = 3f;
			minPick = 199;
			soundType = 21;
		}

        public override bool CanExplode(int i, int j)
        {
            return NPC.downedPlantBoss || CalamityWorld.downedCalamitas;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
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
        }

        public override void RandomUpdate(int i, int j)
        {
            Main.tileValue[Type] = (short)(Main.hardMode ? 690 : 0);
            if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
            {
                int random = WorldGen.genRand.Next(4);
                if (random == 0)
                {
                    i++;
                }
                if (random == 1)
                {
                    i--;
                }
                if (random == 2)
                {
                    j++;
                }
                if (random == 3)
                {
                    j--;
                }
                if (Main.tile[i, j] != null)
                {
                    if (Main.tile[i, j].active())
                    {
                        if (Main.tile[i, j].type == mod.TileType("PlantyMush") && WorldGen.genRand.Next(5) == 0 && !Main.tile[i, j].lava())
                        {
                            Main.tile[i, j].type = (ushort)mod.TileType("Tenebris");
                            WorldGen.SquareTileFrame(i, j, true);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }
    }
}
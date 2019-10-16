using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class ArenaTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Arena");
            AddMapEntry(new Color(128, 0, 0), name);
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
                {
                    WorldGen.KillTile(i, j, false, false, false);
                    if (!Main.tile[i, j].active() && Main.netMode != 0)
                    {
                        NetMessage.SendData(17, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = (float)Main.DiscoR / 255f;
            g = 0f;
            b = 0f;
        }
    }
}

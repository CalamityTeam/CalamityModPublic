using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class Twinkler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkler");
            Main.npcFrameCount[npc.type] = 8;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.LightningBug); //ID is 358
            npc.width = 7;
            npc.height = 5;
            aiType = NPCID.LightningBug;
            animationType = NPCID.LightningBug;
            npc.catchItem = (short)ModContent.ItemType<Items.TwinklerItem>();
            npc.friendly = true; // We have to add this and CanBeHitByItem/CanBeHitByProjectile because of reasons.
                                 //banner = npc.type;
                                 //bannerItem = ModContent.ItemType<Items.TwinklerBanner>();
        }


        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * npc.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * npc.scale;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAstral && !spawnInfo.player.ZoneRockLayerHeight)
            {
                return SpawnCondition.TownCritter.Chance;
            }
            return 0f;
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
                var npcCenter = npc.Center.ToTileCoordinates();
                if (!WorldGen.SolidTile(npcCenter.X, npcCenter.Y) && Main.tile[npcCenter.X, npcCenter.Y].liquid == 0)
                {
                    Main.tile[npcCenter.X, npcCenter.Y].liquid = (byte)Main.rand.Next(50, 150);
                    Main.tile[npcCenter.X, npcCenter.Y].lava(true);
                    Main.tile[npcCenter.X, npcCenter.Y].honey(false);
                    WorldGen.SquareTileFrame(npcCenter.X, npcCenter.Y, true);
                }
            } catch
            {
                return;
            }
        }
    }
}

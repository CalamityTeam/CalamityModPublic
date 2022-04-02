using CalamityMod.Dusts;
using CalamityMod.Items.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
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
            npc.catchItem = (short)ModContent.ItemType<TwinklerItem>();
            npc.friendly = true; // prevents critter from getting slagged
            //banner = npc.type;
            //bannerItem = ModContent.ItemType<TwinklerBanner>();
        }

        public override bool? CanBeHitByItem(Player player, Item item) => true;

        public override bool? CanBeHitByProjectile(Projectile projectile) => true;

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
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral())
            {
                return SpawnCondition.TownCritter.Chance;
            }
            return 0f;
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }
    }
}

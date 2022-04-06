using CalamityMod.Dusts;
using CalamityMod.Items.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Astral
{
    public class Twinkler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkler");
            Main.npcFrameCount[NPC.type] = 8;
            Main.npcCatchable[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.LightningBug); //ID is 358
            NPC.width = 7;
            NPC.height = 5;
            AIType = NPCID.LightningBug;
            AnimationType = NPCID.LightningBug;
            NPC.catchItem = (short)ModContent.ItemType<TwinklerItem>();
            NPC.friendly = true; // prevents critter from getting slagged
            //Banner = npc.type;
            //BannerItem = ModContent.ItemType<TwinklerBanner>();
        }

        public override bool? CanBeHitByItem(Player player, Item item) => true;

        public override bool? CanBeHitByProjectile(Projectile projectile) => true;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * NPC.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * NPC.scale;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral())
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

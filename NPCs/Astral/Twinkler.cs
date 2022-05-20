using CalamityMod.Dusts;
using CalamityMod.Items.Critters;
using Terraria;
using Terraria.GameContent.Bestiary;
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AstralSurface,

                // Will move to localization whenever that is cleaned up.
                new FlavorTextBestiaryInfoElement("A rare case of the astral infection creating a harmless creature. They flicker rather prettily, and you’re not going to be the only one who thinks so. They make useful bait.")
            });
        }

        public override bool? CanBeHitByItem(Player player, Item item) => null;

        public override bool? CanBeHitByProjectile(Projectile projectile) => null;

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

        public override void OnCaughtBy(Player player, Item item, bool failed)
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

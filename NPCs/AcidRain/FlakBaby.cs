using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Pets;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class FlakBaby : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Flak Crab");
            Main.npcFrameCount[NPC.type] = 6;
            Main.npcCatchable[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 32;

            NPC.damage = 0;
            NPC.lifeMax = 5;
            NPC.defense = 5;

            NPC.value = Item.buyPrice(0, 0, 5, 55);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FlakCrabBanner>();
            NPC.dontTakeDamageFromHostiles = true;
            NPC.catchItem = (short)ModContent.ItemType<BabyFlakHermit>();
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextBool(20))
                    NPC.catchItem = (short)ModContent.ItemType<GeyserShell>();
                NPC.localAI[0] = 1f;
                NPC.velocity.Y = -3f;
                NPC.netUpdate = true;
            }
            if (Main.rand.NextBool(8) && NPC.catchItem == (short)ModContent.ItemType<GeyserShell>())
            {
                int dust = Dust.NewDust(NPC.position - new Vector2(2f, 2f), NPC.width + 4, NPC.height + 4, (int)CalamityDusts.SulfurousSeaAcid, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 200, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.1f;
                Main.dust[dust].velocity.Y += 0.25f;
                Main.dust[dust].noLight = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.5f;
                }
            }
            Player closest = Main.player[Player.FindClosest(NPC.Top, 0, 0)];
            if (Math.Abs(closest.Center.X - NPC.Center.X) > 600f)
            {
                NPC.ai[1] = 90;
            }

            if (NPC.ai[1] > 0f)
            {
                NPC.velocity.X *= 0.935f;
                NPC.ai[1]--;
            }
            else
            {
                if (NPC.velocity.Y == 0f && NPC.collideX)
                {
                    NPC.velocity.Y = -13f;
                }
                else
                {
                    NPC.velocity.Y += 0.15f;
                }
                NPC.spriteDirection = (closest.Center.X - NPC.Center.X < 0).ToDirectionInt();
                if (Math.Abs(NPC.velocity.X) < 35f)
                {
                    NPC.velocity.X += NPC.spriteDirection * 0.2f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[1] <= 0f)
            {
                if (NPC.ai[0]++ % 4 == 3)
                {
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (NPC.ai[0]++ % 6 == 5)
                {
                    NPC.frame.Y -= frameHeight;
                }
                if (NPC.frame.Y <= 0)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.Calamity().ZoneSulphur || !DownedBossSystem.downedAquaticScourge)
            {
                return 0f;
            }
            return 0.15f;
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            }
            catch
            {
                return;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/FlakCrab"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/FlakCrab2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/FlakCrab3"), 1f);
            }
        }
    }
}

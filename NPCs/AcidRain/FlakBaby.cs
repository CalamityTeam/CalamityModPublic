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
            Main.npcFrameCount[npc.type] = 6;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.width = 26;
            npc.height = 32;

            npc.damage = 0;
            npc.lifeMax = 5;
            npc.defense = 5;

            npc.value = Item.buyPrice(0, 0, 5, 55);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.DD2_WitherBeastDeath;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FlakCrabBanner>();
            npc.dontTakeDamageFromHostiles = true;
            npc.catchItem = (short)ModContent.ItemType<BabyFlakHermit>();
        }

        public override void AI()
        {
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextBool(20))
                    npc.catchItem = (short)ModContent.ItemType<GeyserShell>();
                npc.localAI[0] = 1f;
                npc.velocity.Y = -3f;
                npc.netUpdate = true;
            }
            if (Main.rand.NextBool(8) && npc.catchItem == (short)ModContent.ItemType<GeyserShell>())
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, (int)CalamityDusts.SulfurousSeaAcid, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 200, default, 1f);
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
            Player closest = Main.player[Player.FindClosest(npc.Top, 0, 0)];
            if (Math.Abs(closest.Center.X - npc.Center.X) > 600f)
            {
                npc.ai[1] = 90;
            }

            if (npc.ai[1] > 0f)
            {
                npc.velocity.X *= 0.935f;
                npc.ai[1]--;
            }
            else
            {
                if (npc.velocity.Y == 0f && npc.collideX)
                {
                    npc.velocity.Y = -13f;
                }
                else
                {
                    npc.velocity.Y += 0.15f;
                }
                npc.spriteDirection = (closest.Center.X - npc.Center.X < 0).ToDirectionInt();
                if (Math.Abs(npc.velocity.X) < 35f)
                {
                    npc.velocity.X += npc.spriteDirection * 0.2f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[1] <= 0f)
            {
                if (npc.ai[0]++ % 4 == 3)
                {
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                {
                    npc.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (npc.ai[0]++ % 6 == 5)
                {
                    npc.frame.Y -= frameHeight;
                }
                if (npc.frame.Y <= 0)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.player.Calamity().ZoneSulphur || !CalamityWorld.downedAquaticScourge)
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
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/FlakCrab"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/FlakCrab2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/FlakCrab3"), 1f);
            }
        }
    }
}

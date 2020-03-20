using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class FlakCrab : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Crab");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.width = 58;
            npc.height = 70;

            npc.damage = 0;
            npc.lifeMax = 700;
            npc.defense = 10;

            if (CalamityWorld.downedPolterghast)
            {
                npc.lifeMax = 7500;
                npc.defense = 80;
            }

            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 5, 55);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.DD2_WitherBeastDeath;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FlakCrabBanner>();
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void AI()
        {
            Player closest = Main.player[Player.FindClosest(npc.Top, 0, 0)];
            if (npc.life / (float)npc.lifeMax > 0.95f)
            {
                if (Math.Abs(closest.Center.X - npc.Center.X) < 320f &&
                    closest.Center.Y - npc.Top.Y < -60f &&
                    npc.ai[1]++ >= Main.rand.Next(35, 95))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = CalamityWorld.downedPolterghast ? 20f : 11f;
                        speed *= Main.rand.NextFloat(0.8f, 1.2f);
                        int damage = CalamityWorld.downedPolterghast ? 42 : 23;
                        Projectile.NewProjectile(npc.Top + Vector2.UnitY * 6f, npc.DirectionTo(closest.Center).RotatedByRandom(0.25f) * speed,
                            ModContent.ProjectileType<FlakAcid>(), damage, 2f);
                        npc.ai[1] = 0;
                    }
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.TargetClosest(true);
                    npc.velocity.X = npc.velocity.X * 0.85f;
                    npc.ai[2]++;
                    float hopRate = 10f + 15f * (npc.life / (float)npc.lifeMax);
                    float lungeForwardSpeed = 10f;
                    float jumpSpeed = 9f;
                    if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        lungeForwardSpeed *= 1.5f;
                    }
                    if (npc.ai[2] > hopRate)
                    {
                        npc.ai[3] += 1f;
                        if (npc.ai[3] >= 3f)
                        {
                            npc.ai[3] = 0f;
                            lungeForwardSpeed *= 1.5f;
                        }
                        npc.ai[2] = 0f;
                        npc.velocity.Y -= jumpSpeed;
                        npc.velocity.X = lungeForwardSpeed * -npc.direction;
                    }
                }
                else
                {
                    npc.knockBackResist = 0f;
                    npc.velocity.X *= 0.995f;
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            // Don't draw the bar if in stealth mode
            if (npc.life / (float)npc.lifeMax > 0.95f)
                return false;
            return null;
        }
        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<CorrodedFossil>(), CalamityWorld.downedAquaticScourge && Main.rand.NextBool(3 * (CalamityWorld.downedPolterghast ? 5 : 1)), 1, 3);
			DropHelper.DropItemChance(npc, ModContent.ItemType<FlakToxicannon>(), 0.05f);
        }
        public override void FindFrame(int frameHeight)
        {
            if (npc.life / (float)npc.lifeMax <= 0.95f)
            {
                if (npc.ai[0]++ % 6 == 5)
                {
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                {
                    npc.frame.Y = frameHeight * 3; // Frames 1 and 2 are for transitioning. Frame 0 is sitting still, and the rest are walking frames
                }
            }
            else
            {
                npc.frame.Y = 0;
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

using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class FlakCrab : ModNPC
    {
        public Player Target => Main.player[npc.target];
        public ref float ChasabilityTimer => ref npc.ai[0];
        public ref float AcidShootTimer => ref npc.ai[1];
        public ref float HopTimer => ref npc.ai[2];
        public ref float HopCounter => ref npc.ai[3];
        public ref float FleeCountdownTimer => ref npc.localAI[0];
        public ref float TotalHits => ref npc.localAI[1];
        public const int TotalHitsNeededToDoDamage = 10;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Crab");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.width = 28;
            npc.height = 70;

            npc.damage = 10;
            npc.lifeMax = 300;

            npc.aiStyle = aiType = -1;

            if (CalamityWorld.downedPolterghast)
            {
                npc.lifeMax = 4125;
                npc.DR_NERD(0.2f);
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 55);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.DD2_WitherBeastDeath;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FlakCrabBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(FleeCountdownTimer);
            writer.Write(TotalHits);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            FleeCountdownTimer = reader.ReadSingle();
            TotalHits = reader.ReadSingle();
        }

        public override void AI()
        {
            // Enables expert scaling, if damage is 0 in set defaults expert scaling will not happen.
            npc.damage = 0;

            ChasabilityTimer++;
            npc.defense = TotalHits < TotalHitsNeededToDoDamage ? 999999 : 20;

            if (npc.justHit)
            {
                FleeCountdownTimer = 240;
                npc.netUpdate = true;
            }

            if (FleeCountdownTimer == 0f || TotalHits < TotalHitsNeededToDoDamage)
            {
                if (ChasabilityTimer < 300f)
                {
                    npc.chaseable = false;
                    npc.knockBackResist = 0f;
                }

                AcidShootTimer++;
                Player closestTargetToTop = Main.player[Player.FindClosest(npc.Top, 0, 0)];
                if (Math.Abs(closestTargetToTop.Center.X - npc.Center.X) < 320f && closestTargetToTop.Center.Y - npc.Top.Y < -60f && AcidShootTimer >= Main.rand.Next(90, 135))
                    ShootFlakAcidAtTarget(closestTargetToTop);
                npc.velocity.X *= 0.97f;
            }
            else
            {
                FleeCountdownTimer--;
                if (npc.velocity.Y == 0f)
                {
                    HopTimer++;
                    npc.knockBackResist = 0.6f;
                    npc.TargetClosest(true);
                    npc.velocity.X *= 0.85f;

                    float hopRate = MathHelper.Lerp(25f, 10f, 1f - npc.life / (float)npc.lifeMax);
                    float lungeForwardSpeed = 10f;
                    float jumpSpeed = 9f;
                    if (Collision.CanHit(npc.Center, 1, 1, Target.Center, 1, 1))
                        lungeForwardSpeed *= 1.5f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && HopTimer > hopRate)
                    {
                        HopCounter++;

                        // Make a bigger leap every 3 hops.
                        if (HopCounter % 3f == 2f)
                            lungeForwardSpeed *= 1.5f;

                        HopTimer = 0f;
                        npc.velocity.Y -= jumpSpeed;
                        npc.velocity.X = lungeForwardSpeed * -npc.direction;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.knockBackResist = 0.2f;
                    npc.velocity.X *= 0.995f;
                }
                npc.chaseable = true;
            }

            if (ChasabilityTimer >= 300f && !npc.chaseable)
                npc.chaseable = true;
        }

        public void ShootFlakAcidAtTarget(Player closestTargetToTop)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float speed = CalamityWorld.downedPolterghast ? 29f : 17f;
            speed *= Main.rand.NextFloat(0.8f, 1.2f);

            int damage = Main.expertMode ? CalamityWorld.downedPolterghast ? 32 : 18 : CalamityWorld.downedPolterghast ? 42 : 23;
            Vector2 spawnPosition = npc.Top + Vector2.UnitY * 6f;
            Vector2 shootVelocity = (closestTargetToTop.Center - spawnPosition).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.25f) * speed;
            Projectile.NewProjectile(spawnPosition, shootVelocity, ModContent.ProjectileType<FlakAcid>(), damage, 2f);

            AcidShootTimer = 0;
            npc.netUpdate = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            // Don't draw the bar if in stealth mode.
            if (FleeCountdownTimer == 0f || TotalHits < TotalHitsNeededToDoDamage)
                return false;
            return null;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<CorrodedFossil>(), 3 * (CalamityWorld.downedPolterghast ? 5 : 1), 1, 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<FlakToxicannon>(), 0.05f);
        }

        public override void FindFrame(int frameHeight)
        {
            if (TotalHits < TotalHitsNeededToDoDamage)
            {
                npc.frame.Y = 0;
                return;
            }

            if (FleeCountdownTimer > 0f)
            {
                if (npc.frameCounter++ % 6 == 5)
                {
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                {
                    npc.frame.Y = frameHeight * 3; // Frames 1 and 2 are for transitioning. Frame 0 is sitting still, and the rest are walking frames
                }
                if (FleeCountdownTimer <= 8)
                    npc.frame.Y = frameHeight;
            }
            else
                npc.frame.Y = 0;
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

            TotalHits++;
            npc.netUpdate = true;
        }
    }
}

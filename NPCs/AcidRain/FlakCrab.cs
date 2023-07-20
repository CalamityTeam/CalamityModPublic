using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class FlakCrab : ModNPC
    {
        public Player Target => Main.player[NPC.target];
        public ref float ChasabilityTimer => ref NPC.ai[0];
        public ref float AcidShootTimer => ref NPC.ai[1];
        public ref float HopTimer => ref NPC.ai[2];
        public ref float HopCounter => ref NPC.ai[3];
        public ref float FleeCountdownTimer => ref NPC.localAI[0];
        public ref float TotalHits => ref NPC.localAI[1];
        public const int TotalHitsNeededToDoDamage = 10;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 70;

            NPC.damage = 10;
            NPC.lifeMax = 300;

            NPC.aiStyle = AIType = -1;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.lifeMax = 4200;
                NPC.DR_NERD(0.2f);
            }

            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 55);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FlakCrabBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FlakCrab")
            });
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
            NPC.damage = 0;

            ChasabilityTimer++;
            NPC.defense = TotalHits < TotalHitsNeededToDoDamage ? 999999 : 20;

            if (NPC.justHit)
            {
                FleeCountdownTimer = 240;
                NPC.netUpdate = true;
            }

            if (FleeCountdownTimer == 0f || TotalHits < TotalHitsNeededToDoDamage)
            {
                if (ChasabilityTimer < 300f)
                {
                    NPC.chaseable = false;
                    NPC.knockBackResist = 0f;
                }

                AcidShootTimer++;
                Player closestTargetToTop = Main.player[Player.FindClosest(NPC.Top, 0, 0)];
                if (Math.Abs(closestTargetToTop.Center.X - NPC.Center.X) < 320f && closestTargetToTop.Center.Y - NPC.Top.Y < -60f && AcidShootTimer >= Main.rand.Next(90, 135))
                    ShootFlakAcidAtTarget(closestTargetToTop);
                NPC.velocity.X *= 0.97f;
            }
            else
            {
                FleeCountdownTimer--;
                if (NPC.velocity.Y == 0f)
                {
                    HopTimer++;
                    NPC.knockBackResist = 0.6f;
                    NPC.TargetClosest(true);
                    NPC.velocity.X *= 0.85f;

                    float hopRate = MathHelper.Lerp(25f, 10f, 1f - NPC.life / (float)NPC.lifeMax);
                    float lungeForwardSpeed = 10f;
                    float jumpSpeed = 9f;
                    if (Collision.CanHit(NPC.Center, 1, 1, Target.Center, 1, 1))
                        lungeForwardSpeed *= 1.5f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && HopTimer > hopRate)
                    {
                        HopCounter++;

                        // Make a bigger leap every 3 hops.
                        if (HopCounter % 3f == 2f)
                            lungeForwardSpeed *= 1.5f;

                        HopTimer = 0f;
                        NPC.velocity.Y -= jumpSpeed;
                        NPC.velocity.X = lungeForwardSpeed * -NPC.direction;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    NPC.knockBackResist = 0.2f;
                    NPC.velocity.X *= 0.995f;
                }
                NPC.chaseable = true;
            }

            if (ChasabilityTimer >= 300f && !NPC.chaseable)
                NPC.chaseable = true;
        }

        public void ShootFlakAcidAtTarget(Player closestTargetToTop)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float speed = DownedBossSystem.downedPolterghast ? 29f : 17f;
            speed *= Main.rand.NextFloat(0.8f, 1.2f);

            int damage = Main.expertMode ? DownedBossSystem.downedPolterghast ? 32 : 18 : DownedBossSystem.downedPolterghast ? 42 : 23;
            Vector2 spawnPosition = NPC.Top + Vector2.UnitY * 6f;
            Vector2 shootVelocity = (closestTargetToTop.Center - spawnPosition).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.25f) * speed;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, shootVelocity, ModContent.ProjectileType<FlakAcid>(), damage, 2f);

            AcidShootTimer = 0;
            NPC.netUpdate = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            // Don't draw the bar if in stealth mode.
            if (FleeCountdownTimer == 0f || TotalHits < TotalHitsNeededToDoDamage)
                return false;
            return null;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<FlakToxicannon>(), 20);
            LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast);
            postPolter.Add(ModContent.ItemType<CorrodedFossil>(), 15, 1, 3, !DownedBossSystem.downedPolterghast);
            postPolter.AddFail(ModContent.ItemType<CorrodedFossil>(), 3, 1, 3, DownedBossSystem.downedPolterghast);
        }

        public override void FindFrame(int frameHeight)
        {
            if (TotalHits < TotalHitsNeededToDoDamage && !NPC.IsABestiaryIconDummy)
            {
                NPC.frame.Y = 0;
                return;
            }

            if (FleeCountdownTimer > 0f || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frameCounter++ % 6 == 5)
                {
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.frame.Y = frameHeight * 3; // Frames 1 and 2 are for transitioning. Frame 0 is sitting still, and the rest are walking frames
                }
                if (FleeCountdownTimer <= 8 && !NPC.IsABestiaryIconDummy)
                    NPC.frame.Y = frameHeight;
            }
            else
                NPC.frame.Y = 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("FlakCrabGore1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("FlakCrabGore2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("FlakCrabGore3").Type, 1f);
                }
            }

            TotalHits++;
            NPC.netUpdate = true;
        }
    }
}

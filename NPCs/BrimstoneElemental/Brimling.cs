using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.BrimstoneElemental
{
    public class Brimling : ModNPC
    {
        private bool boostDR = false;
        public static float normalDR = 0.15f;
        public static float boostedDR = 0.6f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 50;
            NPC.width = 60;
            NPC.height = 60;
            NPC.defense = 0;
            NPC.DR_NERD(normalDR);
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath39;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };

            if (Main.zenithWorld)
                NPC.scale *= 0.7f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<BrimstoneElemental>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Brimling")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(boostDR);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            boostDR = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);

            // Despawn if Brim doesn't exist
            if (CalamityGlobalNPC.brimstoneElemental < 0 || !Main.npc[CalamityGlobalNPC.brimstoneElemental].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Variables for buffing the AI
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining for Brim
            float lifeRatio = Main.npc[CalamityGlobalNPC.brimstoneElemental].life / (float)Main.npc[CalamityGlobalNPC.brimstoneElemental].lifeMax;

            // Enraged Brim checks
            bool biomeEnraged = Main.npc[CalamityGlobalNPC.brimstoneElemental].Calamity().newAI[3] <= 0f || bossRush;
            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && (!Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].ZoneUnderworldHeight || bossRush))
            {
                Main.npc[CalamityGlobalNPC.brimstoneElemental].Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (biomeEnraged && (!Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].Calamity().ZoneCalamity || bossRush))
            {
                Main.npc[CalamityGlobalNPC.brimstoneElemental].Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }

            // Brim phase checks
            bool brimIsAboutToTeleport = Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] == 1f && Main.npc[CalamityGlobalNPC.brimstoneElemental].alpha == 0;
            bool brimIsFlyingAboveAndShooting = Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] == 3f;
            bool brimIsInCocoon = Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] == 4f;
            bool brimIsFiringLaser = Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] == 5f;

            // Set alpha to Brim alpha
            NPC.alpha = Main.npc[CalamityGlobalNPC.brimstoneElemental].alpha;

            // Go into cocoon if Brim is in her cocoon
            if (brimIsInCocoon)
            {
                boostDR = true;
                NPC.chaseable = false;
            }
            else
            {
                boostDR = false;
                NPC.chaseable = true;
            }

            // Set DR based on boost status
            NPC.Calamity().DR = boostDR ? boostedDR : normalDR;

            // Brimlings only shoot if Brim is shooting
            if (brimIsFlyingAboveAndShooting || brimIsFiringLaser)
            {
                float shootDivisor = death ? 45f : 60f;
                if (brimIsFlyingAboveAndShooting)
                {
                    float divisor = (death ? 80f : 45f) - (float)Math.Ceiling(10f * (1f - lifeRatio));
                    divisor -= 5f * enrageScale;
                    shootDivisor = divisor * 2f;
                }

                NPC.ai[1] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] % shootDivisor == 0f)
                {
                    float projectileVelocity = 5f;
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = NPC.GetProjectileDamage(type);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].Center - NPC.Center) * projectileVelocity, type, damage, 0f, Main.myPlayer, 1f, 0f);
                }
            }
            else
                NPC.ai[1] = 0f;

            // Face Brim's target
            if (Math.Abs(NPC.Center.X - Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].Center.X) > 10f)
            {
                float playerLocation = NPC.Center.X - Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }

            // Rotate slightly
            NPC.rotation = Math.Abs(NPC.velocity.X) * NPC.direction * 0.05f;

            // Move towards Brim
            float movementVelocity = (death ? 12f : 10f) * (brimIsFlyingAboveAndShooting ? 1.5f : boostDR ? 0.5f : 1f);
            movementVelocity += 5f * enrageScale;
            Vector2 distanceFromMother = Main.npc[CalamityGlobalNPC.brimstoneElemental].Center - NPC.Center;
            if (distanceFromMother.Length() > 120f)
            {
                Vector2 farFromBrimVelocityMult = distanceFromMother;
                if (farFromBrimVelocityMult.Length() > movementVelocity)
                {
                    farFromBrimVelocityMult.Normalize();
                    farFromBrimVelocityMult *= movementVelocity;
                }
                int inertia = 20;
                NPC.velocity = (NPC.velocity * (inertia - 1) + farFromBrimVelocityMult) / inertia;
            }
            else
                NPC.velocity *= 0.96f;

            // Push away from other Brimlings
            float pushVelocity = 0.5f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                    {
                        if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 40f * NPC.scale)
                        {
                            if (NPC.position.X < Main.npc[i].position.X)
                                NPC.velocity.X -= pushVelocity;
                            else
                                NPC.velocity.X += pushVelocity;

                            if (NPC.position.Y < Main.npc[i].position.Y)
                                NPC.velocity.Y -= pushVelocity;
                            else
                                NPC.velocity.Y += pushVelocity;
                        }
                    }
                }
            }

            // Teleport to Brim's new location
            if (NPC.ai[2] != 0f && NPC.ai[3] != 0f)
            {
                for (int i = 0; i < 20; i++)
                {
                    int deepRedDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 235, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[deepRedDust];
                    dust.velocity *= 3f;
                    Main.dust[deepRedDust].noGravity = true;
                    Main.dust[deepRedDust].scale = 2.5f;
                }

                NPC.Center = new Vector2(NPC.ai[2] * 16f, NPC.ai[3] * 16f);
                NPC.velocity = Vector2.Zero;

                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;

                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                for (int j = 0; j < 20; j++)
                {
                    int deepRedDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 235, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[deepRedDust2];
                    dust.velocity *= 3f;
                    Main.dust[deepRedDust2].noGravity = true;
                    Main.dust[deepRedDust2].scale = 2.5f;
                }
            }

            // Shoot a fireball when Brim is about to teleport
            // Shoot before she goes invisible to make it as fair as possible
            if (brimIsAboutToTeleport && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float projectileVelocity = 5f;
                int type = ModContent.ProjectileType<BrimstoneHellfireball>();
                int damage = NPC.GetProjectileDamage(type);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].Center - NPC.Center) * projectileVelocity, type, damage, 0f, Main.myPlayer, Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].position.X, Main.player[Main.npc[CalamityGlobalNPC.brimstoneElemental].target].position.Y);
            }

            // Teleport when Brim teleports
            if (Main.npc[CalamityGlobalNPC.brimstoneElemental].alpha == 255 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Point npcTileCoords = NPC.Center.ToTileCoordinates();
                Point brimmyTileCoords = Main.npc[CalamityGlobalNPC.brimstoneElemental].Center.ToTileCoordinates();
                int babTeleportRadius = 3;
                int brimmyTeleportRadius = 4;
                int solidTileRadius = 1;
                int increment = 0;
                while (increment < 100)
                {
                    increment++;
                    int randXOffset = Main.rand.Next(brimmyTileCoords.X - 6, brimmyTileCoords.X + 7);
                    int randYOffset = Main.rand.Next(brimmyTileCoords.Y - 6, brimmyTileCoords.Y + 7);
                    if ((randYOffset < brimmyTileCoords.Y - brimmyTeleportRadius || randYOffset > brimmyTileCoords.Y + brimmyTeleportRadius || randXOffset < brimmyTileCoords.X - brimmyTeleportRadius || randXOffset > brimmyTileCoords.X + brimmyTeleportRadius) && (randYOffset < npcTileCoords.Y - babTeleportRadius || randYOffset > npcTileCoords.Y + babTeleportRadius || randXOffset < npcTileCoords.X - babTeleportRadius || randXOffset > npcTileCoords.X + babTeleportRadius) && !Main.tile[randXOffset, randYOffset].HasUnactuatedTile)
                    {
                        bool canTeleport = true;
                        if (canTeleport && Main.tile[randXOffset, randYOffset].LiquidType == LiquidID.Lava)
                            canTeleport = false;
                        if (canTeleport && Collision.SolidTiles(randXOffset - solidTileRadius, randXOffset + solidTileRadius, randYOffset - solidTileRadius, randYOffset + solidTileRadius))
                            canTeleport = false;

                        if (canTeleport)
                        {
                            NPC.ai[2] = randXOffset;
                            NPC.ai[3] = randYOffset;
                            break;
                        }
                    }
                }

                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (!boostDR)
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (NPC.frame.Y >= frameHeight * 4)
                    NPC.frame.Y = 0;
            }
            else
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (NPC.frame.Y < frameHeight * 4)
                    NPC.frame.Y = frameHeight * 4;

                if (NPC.frame.Y >= frameHeight * 8)
                    NPC.frame.Y = frameHeight * 4;
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}

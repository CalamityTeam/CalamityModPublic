using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [LongDistanceNetSync]
    public class SepulcherHead : ModNPC
    {
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/SepulcherDeath");

        private const int minLength = 51;
        private const int maxLength = 52;
        private float passedVar = 0f;
        private bool TailSpawned = false;
        private float AttackCooldown = 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionXOverride = 30,
                PortraitPositionYOverride = 0,
                PortraitScale = 0.54f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/Sepulcher_Bestiary"
            };
            value.Position.X += 80;
            value.Position.Y -= 13;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.npcSlots = 5f;
            NPC.width = 62;
            NPC.height = 64;
            NPC.defense = 0;
            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.scale *= Main.expertMode ? 1.35f : 1.2f;
            NPC.scale *= 1.25f;
            NPC.alpha = 255;
            NPC.chaseable = false;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.netAlways = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<SupremeCalamitas>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Sepulcher")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.alpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.alpha = reader.ReadInt32();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (AttackCooldown > 0)
                AttackCooldown--;
            CalamityGlobalNPC.SCalWorm = NPC.whoAmI;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && NPC.ai[0] == 0f)
                {
                    float rotationalOffset = 0f;
                    int Previous = NPC.whoAmI;
                    for (int i = 0; i < maxLength; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength && i % 2 == 1)
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<SepulcherBodyEnergyBall>(), NPC.whoAmI);
                            Main.npc[lol].localAI[0] += passedVar;
                            passedVar += 36f;
                        }
                        else if (i >= 0 && i < minLength)
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<SepulcherBody>(), NPC.whoAmI);
                            Main.npc[lol].localAI[3] = i;
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<SepulcherTail>(), NPC.whoAmI);

                        // Create arms.
                        if (i >= 3 && i % 4 == 0)
                        {
                            NPC segment = Main.npc[lol];
                            int arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)segment.Center.X, (int)segment.Center.Y, ModContent.NPCType<SepulcherArm>(), lol);
                            if (Main.npc.IndexInRange(arm))
                            {
                                Main.npc[arm].ai[0] = lol;
                                Main.npc[arm].direction = 1;
                                Main.npc[arm].rotation = rotationalOffset;
                            }

                            rotationalOffset += MathHelper.Pi / 6f;

                            arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)segment.Center.X, (int)segment.Center.Y, ModContent.NPCType<SepulcherArm>(), lol);
                            if (Main.npc.IndexInRange(arm))
                            {
                                Main.npc[arm].ai[0] = lol;
                                Main.npc[arm].direction = -1;
                                Main.npc[arm].rotation = rotationalOffset + MathHelper.Pi;
                            }

                            rotationalOffset += MathHelper.Pi / 6f;
                            rotationalOffset = MathHelper.WrapAngle(rotationalOffset);
                        }

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (!NPC.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            if (Main.zenithWorld && !NPC.AnyNPCs(ModContent.NPCType<BrimstoneHeart>()))
            {
                CalamityGlobalNPC global = NPC.Calamity();
                global.DR = 0.4f;
                global.unbreakableDR = false;
                NPC.chaseable = true;
                NPC.DeathSound = DeathSound;
            }

            if (Main.player[NPC.target].dead || (!NPC.AnyNPCs(ModContent.NPCType<BrimstoneHeart>()) && !Main.zenithWorld) || CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                NPC.TargetClosest(false);
                SoundEngine.PlaySound(DeathSound, Main.player[NPC.target].Center);
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }
            else
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.165f, 0f, 1f);

            Vector2 segmentLocation = NPC.Center;
            float targetX = CalamityGlobalNPC.SCal < 0 ? 0 : Main.npc[CalamityGlobalNPC.SCal].position.X + (Main.player[NPC.target].width / 2);
            float targetY = CalamityGlobalNPC.SCal < 0 ? 0 : Main.npc[CalamityGlobalNPC.SCal].position.Y + (Main.player[NPC.target].height / 2);
            float sepMaxSpeed = BossRushEvent.BossRushActive ? 22.5f : 20f;
            float sepAcceleration = (BossRushEvent.BossRushActive ? 0.2f : 0.175f) + (0.37f - AttackCooldown * 0.0015f);

            float fasterMaxSpeed = sepMaxSpeed * 1.3f;
            float slowerMaxSpeed = sepMaxSpeed * 0.7f;
            float currentSpeed = NPC.velocity.Length();
            if (currentSpeed > 0f)
            {
                if (currentSpeed > fasterMaxSpeed)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= fasterMaxSpeed;
                }
                else if (currentSpeed < slowerMaxSpeed)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= slowerMaxSpeed;
                }
            }

            targetX = (int)(targetX / 16f) * 16;
            targetY = (int)(targetY / 16f) * 16;
            segmentLocation.X = (int)(segmentLocation.X / 16f) * 16;
            segmentLocation.Y = (int)(segmentLocation.Y / 16f) * 16;
            targetX -= segmentLocation.X;
            targetY -= segmentLocation.Y;
            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
            float absoluteTargetX = Math.Abs(targetX);
            float absoluteTargetY = Math.Abs(targetY);
            float timeToReachTarget = sepMaxSpeed / targetDistance;
            targetX *= timeToReachTarget;
            targetY *= timeToReachTarget;
            if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f) || (NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f))
            {
                if (NPC.velocity.X < targetX)
                {
                    NPC.velocity.X = NPC.velocity.X + sepAcceleration;
                }
                else
                {
                    if (NPC.velocity.X > targetX)
                    {
                        NPC.velocity.X = NPC.velocity.X - sepAcceleration;
                    }
                }
                if (NPC.velocity.Y < targetY)
                {
                    NPC.velocity.Y = NPC.velocity.Y + sepAcceleration;
                }
                else
                {
                    if (NPC.velocity.Y > targetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - sepAcceleration;
                    }
                }
                if (Math.Abs(targetY) < sepMaxSpeed * 0.2 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                {
                    if (NPC.velocity.Y > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + sepAcceleration * 2f;
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y - sepAcceleration * 2f;
                    }
                }
                if (Math.Abs(targetX) < sepMaxSpeed * 0.2 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
                {
                    if (NPC.velocity.X > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + sepAcceleration * 2f; //changed from 2
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - sepAcceleration * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (absoluteTargetX > absoluteTargetY)
                {
                    if (NPC.velocity.X < targetX)
                    {
                        NPC.velocity.X = NPC.velocity.X + sepAcceleration * 1.1f; //changed from 1.1
                    }
                    else if (NPC.velocity.X > targetX)
                    {
                        NPC.velocity.X = NPC.velocity.X - sepAcceleration * 1.1f; //changed from 1.1
                    }
                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < sepMaxSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + sepAcceleration;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - sepAcceleration;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + sepAcceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y > targetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - sepAcceleration * 1.1f;
                    }
                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < sepMaxSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + sepAcceleration;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - sepAcceleration;
                        }
                    }
                }
            }
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
            float targetDist = Vector2.Distance(NPC.Center, Main.npc[CalamityGlobalNPC.SCal].Center);
            if (targetDist <= 110 && AttackCooldown <= 0)
            {
                AttackCooldown = 150;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = (int)(NPC.GetProjectileDamage(type) * 0.5f);
                    int totalProjectiles = 30;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    float velocity = 1f;
                    float projectileVelocityToPass = 15f;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-velocity, -velocity));
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 projectileVelocity = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 3f, projectileVelocityToPass);
                    }
                    NPC.netUpdate = true;
                }

                Particle bloom = new BloomParticle(NPC.Center, Vector2.Zero, Color.Red, 0.1f, 0.9f, 30, false);
                GeneralParticleHandler.SpawnParticle(bloom);
                Particle bloom2 = new BloomParticle(NPC.Center, Vector2.Zero, Color.White, 0.1f, 0.8f, 30, false);
                GeneralParticleHandler.SpawnParticle(bloom2);

                Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(2f, 2f), 0, 0f, 0.9f, 25);
                GeneralParticleHandler.SpawnParticle(pulse);

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/DeadSunRicochet") with { Pitch = -0.65f, Volume = 1.8f }, NPC.Center);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // hit sound in gfb
            if (NPC.soundDelay == 0 && NPC.Calamity().unbreakableDR == false)
            {
                NPC.soundDelay = Main.rand.Next(5, 8);
                SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, NPC.Center);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Vector2 goreSpawnPosition = NPC.Center;

                        // Spawn at a slight offset when spawning mandibles.
                        if (i == 2)
                            goreSpawnPosition += NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver4) * 16f;
                        if (i == 3)
                            goreSpawnPosition += NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver4) * 16f;
                        Gore.NewGorePerfect(NPC.GetSource_Death(), goreSpawnPosition, NPC.velocity, Mod.Find<ModGore>($"SepulcherHead_Gore{i}").Type, NPC.scale);
                    }
                }
            }
        }
    }
}

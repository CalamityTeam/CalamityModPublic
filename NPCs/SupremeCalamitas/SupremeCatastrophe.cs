using System;
using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [AutoloadBossHead]
    public class SupremeCatastrophe : ModNPC
    {
        public int VerticalOffset = 0;
        public int CurrentFrame;
        public bool SlashingFromRight;
        public int HorizontalOffset = 750;
        public const int SlashCounterLimit = 50;
        public const int DartBurstCounterLimit = 300;
        public const int PreBigAttackPause = 50;
        public int BigAttackLimit = 7;
        public bool targetSide = false;
        public int dashAttackTimer = 0;
        public int dashes = 0;
        public Vector2 offset = Vector2.Zero;
        public bool MovingUp = false;
        public bool EnrageRoar = true;
        public int accSlashCounter = 0;
        public bool setMovement = true;
        public bool broIsAlive = true;
        public Player Target => Main.player[NPC.target];
        public ref float SlashCounter => ref NPC.ai[1];
        public ref float DartBurstCounter => ref NPC.ai[2];
        public ref float ElapsedVerticalDistance => ref NPC.ai[3];
        public ref float AttackDelayTimer => ref NPC.localAI[0];
        public ref float BigAttackTimer => ref NPC.localAI[1];

        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.3f,
                PortraitPositionYOverride = 56f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.damage = 50;
            NPC.npcSlots = 5f;
            NPC.width = 120;
            NPC.height = 120;
            NPC.defense = 80;
            NPC.DR_NERD(SupremeCataclysm.NormalBrothersDR);
            NPC.lifeMax = 138000;
            double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SupremeCalamitas.BrotherHit;
            NPC.DeathSound = SupremeCalamitas.BrotherDeath;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.localAI[1] = 500;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<SupremeCalamitas>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SupremeCatastrophe")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(VerticalOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            VerticalOffset = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
            float slashCounter = SlashCounter + (SlashingFromRight ? 0f : SlashCounterLimit);
            float slashInterpolant = Utils.GetLerpValue(0f, SlashCounterLimit * 2f, slashCounter, true);
            if (AttackDelayTimer < 120f)
            {
                NPC.frameCounter += 0.15f;
                if (NPC.frameCounter >= 1f)
                {
                    CurrentFrame = (CurrentFrame + 1) % 6;
                    NPC.frameCounter = 0f;
                }
            }
            else
            {
                CurrentFrame = (int)Math.Round(MathHelper.Lerp(6f, 15f, slashInterpolant));
            }

            int xFrame = CurrentFrame / Main.npcFrameCount[NPC.type];
            int yFrame = CurrentFrame % Main.npcFrameCount[NPC.type];

            NPC.frame.Width = 400;
            NPC.frame.Height = 230;
            NPC.frame.X = xFrame * NPC.frame.Width;
            NPC.frame.Y = yFrame * NPC.frame.Height;
        }

        public override void AI()
        {
            if (setMovement)
            {
                MovingUp = (NPC.ai[0] == 1 ? true : false);
                setMovement = false;
            }

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            NPC.direction = NPC.spriteDirection;

            if (BigAttackTimer > 0)
                BigAttackTimer--;

            // Set the whoAmI variable.
            CalamityGlobalNPC.SCalCatastrophe = NPC.whoAmI;

            // Disappear if Supreme Calamitas is not present.
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            NPC scal = Main.npc[CalamityGlobalNPC.SCal];
            if (scal.ModNPC<SupremeCalamitas>().respawnBro == false && Main.masterMode && !broIsAlive)
            {
                if (NPC.life > (NPC.lifeMax * 0.65f))
                    NPC.life = (int)(NPC.lifeMax * 0.65f);
            }

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Increase DR if the target leaves SCal's arena.
            NPC.Calamity().DR = SupremeCataclysm.NormalBrothersDR;
            if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().IsTargetOutsideOfArena)
                NPC.Calamity().DR = SupremeCalamitas.enragedDR;

            float totalLifeRatio = NPC.life / (float)NPC.lifeMax;
            if (CalamityGlobalNPC.SCalCataclysm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCalCataclysm].active)
                    totalLifeRatio += Main.npc[CalamityGlobalNPC.SCalCataclysm].life / (float)Main.npc[CalamityGlobalNPC.SCalCataclysm].lifeMax;
            }
            totalLifeRatio *= 0.5f;

            // Get a target if no valid one has been found.
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Target.dead || !Target.active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away.
            if (!NPC.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance200Tiles))
                NPC.TargetClosest();

            float acceleration = Utils.Remap(AttackDelayTimer, 0f, 120f, 2f, 4f);
            int verticalSpeed = (int)Math.Round(MathHelper.Lerp(2f, 6.5f, 1f - totalLifeRatio));

            bool Phase2 = broIsAlive == false ? true : false;
            if (NPC.life / (float)NPC.lifeMax < (death ? 0.6 : 0.4) && !Phase2)
            {
                if (EnrageRoar)
                {
                    EnrageRoar = false;
                    SoundStyle yell = new("CalamityMod/Sounds/NPCKilled/RavagerLimbLoss2");
                    SoundEngine.PlaySound(yell with { Volume = 1.5f, Pitch = 0.3f }, NPC.Center);
                }
                Phase2 = true;
            }


            // Buffs for the big attack when the other brother dies
            if (Phase2 && BigAttackTimer > 400)
            BigAttackTimer = 400;

            if (Phase2 && BigAttackTimer > 0 && BigAttackLimit < 11)
                BigAttackLimit = 11;
            else if (BigAttackTimer > 0 && BigAttackLimit < 7)
                BigAttackLimit = 7;

            if (BigAttackTimer > PreBigAttackPause)
            {
                if (Phase2)
                {
                    if (MovingUp)
                    {
                        // Move up.
                        if (VerticalOffset < 400)
                        {
                            VerticalOffset += (int)(verticalSpeed * 1.5f);
                        }
                        else
                            MovingUp = false;
                    }
                    else
                    {
                        // Move down.
                        if (VerticalOffset > -400)
                        {
                            VerticalOffset -= (int)(verticalSpeed * 1.5f);
                        }
                        else
                            MovingUp = true;
                    }

                    // Hover to the side of the target.
                    Vector2 idealVelocity = NPC.SafeDirectionTo(Target.Center + new Vector2(-HorizontalOffset * (targetSide ? -1 : 1), VerticalOffset)) * (Phase2 ? Utils.Remap(AttackDelayTimer, 0f, 120f, 15f, 50f) : Utils.Remap(AttackDelayTimer, 60f, 120f, 0f, 50f));
                    if (SlashCounter <= SlashCounterLimit * 0.3f && dashAttackTimer == 0)
                    {
                        if (AttackDelayTimer == 120 && Main.rand.NextBool())
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Dust catastrophedust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height) - NPC.velocity * 0.5f, 66, -NPC.velocity * Main.rand.NextFloat(0.1f, 0.6f));
                                catastrophedust.noGravity = true;
                                catastrophedust.scale = Main.rand.NextFloat(0.7f, 1.3f);
                                catastrophedust.color = Color.Cyan;
                            }
                        }
                        CalamityUtils.SmoothMovement(NPC, 0, (Target.Center + new Vector2(-HorizontalOffset * (!targetSide ? 1 : -1), VerticalOffset)) - NPC.Center, Utils.Remap(AttackDelayTimer, 10f, 120f, 20f, 80f), 1f, false);
                    }
                    else if (dashAttackTimer == 0)
                        NPC.velocity *= 0.85f;
                    else
                    {
                        dashAttackTimer--;

                        int type = ModContent.ProjectileType<SupremeCatastropheSlash>();
                        int damage = NPC.GetProjectileDamage(type);
                        if (bossRush)
                            damage /= 2;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.UnitY) * 0.1f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt(), 4 + dashes);
                        dashes++;
                        if (dashes == 30 && broIsAlive == false)
                        {
                            if (Main.zenithWorld)
                            {
                                SoundStyle charge = new("CalamityMod/Sounds/Item/ExobladeBeamSlash");
                                SoundEngine.PlaySound(charge with { Volume = 0.85f, Pitch = -0.5f }, NPC.Center);
                                NPC.velocity = NPC.DirectionTo(Target.Center) * 150f;

                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 vel = new Vector2(7, 7).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                                    Dust catastrophedust = Dust.NewDustPerfect(NPC.Center + vel * 2, 279, vel);
                                    catastrophedust.noGravity = true;
                                    catastrophedust.scale = Main.rand.NextFloat(1.2f, 1.8f);
                                    catastrophedust.color = Color.DeepSkyBlue;
                                }
                                dashes = 0;
                                dashAttackTimer = 30;
                                BigAttackTimer = 500;
                                AttackDelayTimer = 120;
                                if (NPC.life > 10001)
                                    NPC.life -= 10000;
                                return;
                            }
                            else
                            {
                                SoundStyle slash = new("CalamityMod/Sounds/Item/MurasamaBigSwing");
                                SoundEngine.PlaySound(slash with { Volume = 0.55f, Pitch = -0.3f }, NPC.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.DirectionTo(Target.Center).RotatedBy(0.34f) * 130, NPC.DirectionTo(Target.Center).RotatedBy(0.34f) * 90f, type, damage, 0f, Main.myPlayer, 0f, 0, 50);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.DirectionTo(Target.Center).RotatedBy(-0.34f) * 130, NPC.DirectionTo(Target.Center).RotatedBy(-0.34f) * 90f, type, damage, 0f, Main.myPlayer, 0f, 0, 50);
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 vel = new Vector2(7, 7).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                                    Dust catastrophedust = Dust.NewDustPerfect(NPC.Center + vel * 2, 279, vel);
                                    catastrophedust.noGravity = true;
                                    catastrophedust.scale = Main.rand.NextFloat(1.2f, 1.8f);
                                    catastrophedust.color = Color.DeepSkyBlue;
                                }
                            }
                        }
                        NPC.velocity *= 0.975f;
                    }
                }
                else
                {
                    if (MovingUp)
                    {
                        // Move up.
                        if (VerticalOffset < 400)
                        {
                            VerticalOffset += verticalSpeed;
                        }
                        else
                            MovingUp = false;
                    }
                    else
                    {
                        // Move down.
                        if (VerticalOffset > -400)
                        {
                            VerticalOffset -= verticalSpeed;
                        }
                        else
                            MovingUp = true;
                    }

                    if (SlashCounter <= SlashCounterLimit * 0.3f)
                    {
                        offset = NPC.DirectionTo(Target.Center) * 70f * (BigAttackTimer <= 60 ? -4 : 1);
                    }
                    else
                        offset *= 0.9f;
                    CalamityUtils.SmoothMovement(NPC, 0, (Target.Center + new Vector2(-HorizontalOffset * (!targetSide ? 1 : -1), VerticalOffset)) - NPC.Center - offset, 17, 2f, false);
                }
                
            }
            else
                NPC.velocity *= 0.9f;   

            // Reset rotation to zero.
            NPC.rotation = 0f;

            // Set direction.
            NPC.spriteDirection = (Target.Center.X > NPC.Center.X).ToDirectionInt();

            // Have a small delay prior to shooting projectiles.
            if (AttackDelayTimer < 120f)
                AttackDelayTimer += (death ? 1.5f : 1f);

            // Handle projectile shots.
            else if (BigAttackTimer > PreBigAttackPause)
            {
                // Shoot sword slashes.
                float fireRate = BossRushEvent.BossRushActive ? 2f : MathHelper.Lerp(1.5f, 2f, 1f - totalLifeRatio) * (broIsAlive == false ? death ? 1.25f : 1.05f : 1);
                SlashCounter += fireRate;
                if (SlashCounter >= SlashCounterLimit)
                {
                    SlashCounter = 0f;
                    SoundEngine.PlaySound(SupremeCalamitas.CatastropheSwing with { Volume = 0.5f, Pitch = (SlashingFromRight ? 0.2f : -0.2f) }, NPC.Center);

                    int type = ModContent.ProjectileType<SupremeCatastropheSlash>();

                    if (Main.zenithWorld)
                        type = ModContent.ProjectileType<SupremeCataclysmFist>();

                    int damage = NPC.GetProjectileDamage(type);
                    if (bossRush)
                        damage /= 2;
                    Vector2 slashSpawnPosition = NPC.Center + Vector2.UnitX * 125f * NPC.direction;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    { 
                        Vector2 firingVelocity = (broIsAlive == false ? (NPC.DirectionTo(Target.Center) + Target.velocity * 0.0305f).SafeNormalize(Vector2.UnitY) : NPC.DirectionTo(Target.Center));
                        if (Phase2)
                        {
                            if (accSlashCounter < 2)
                            {
                                accSlashCounter++;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), slashSpawnPosition, firingVelocity * 5.5f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt(), 2);
                            }
                            else
                            {
                                SoundStyle slash = new("CalamityMod/Sounds/Item/MurasamaBigSwing");
                                SoundEngine.PlaySound(slash with { Volume = 0.55f, Pitch = 0.4f }, NPC.Center);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), slashSpawnPosition, firingVelocity * 0.2f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt(), Main.zenithWorld ? 0 : 3);
                                accSlashCounter = 0;
                            }
                        }
                        else
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), slashSpawnPosition, NPC.DirectionTo(Target.Center) * 8f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt(), 2);
                    }
                    SlashingFromRight = !SlashingFromRight;
                    CurrentFrame = 0;
                    broIsAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>());
                }
            }
            // Pause before attacking
            else if (BigAttackTimer > 0)
            {
                if (BigAttackTimer == PreBigAttackPause)
                {
                    SoundStyle charge = new("CalamityMod/Sounds/Custom/Ravager/RavagerPillarSummon");
                    SoundEngine.PlaySound(charge with { Volume = 0.85f, Pitch = 0.6f }, NPC.Center);
                }
                for (int i = 0; i < 7; i++)
                {
                    Vector2 vel = new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                    Dust catastrophedust = Dust.NewDustPerfect(NPC.Center + vel * 2, 279, vel);
                    catastrophedust.noGravity = true;
                    catastrophedust.scale = Main.rand.NextFloat(1.2f, 1.8f);
                    catastrophedust.color = Color.DeepSkyBlue;
                }

                BigAttackTimer--;
            }
            // Big attack
            else
            {
                // Shoot sword slashes.
                float fireRate = BossRushEvent.BossRushActive ? 2.8f : MathHelper.Lerp(2.5f, 3f, 1f - totalLifeRatio) * (broIsAlive == false ? 1.35f : 1);
                if (Phase2 && BigAttackLimit == 0)
                    fireRate = 1;
                SlashCounter += fireRate;
                if (SlashCounter >= SlashCounterLimit)
                {
                    SlashCounter = 0f;

                    int type = ModContent.ProjectileType<SupremeCatastropheSlash>();

                    if (Main.zenithWorld)
                        type = ModContent.ProjectileType<SupremeCataclysmFist>();

                    int damage = NPC.GetProjectileDamage(type);
                    if (bossRush)
                        damage /= 2;
                    Vector2 slashSpawnPosition = NPC.Center;
                    Vector2 firingVelocity = (broIsAlive == false ? (NPC.DirectionTo(Target.Center) + Target.velocity * 0.032f).SafeNormalize(Vector2.UnitY) : NPC.DirectionTo(Target.Center));

                    if ((broIsAlive == false ? BigAttackLimit <= 3 && BigAttackLimit > 0 : BigAttackLimit == 1) && death)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), slashSpawnPosition + firingVelocity * 130, (Phase2 ? NPC.DirectionTo(Target.Center) : firingVelocity) * 90f, ModContent.ProjectileType<SupremeCatastropheSlash>(), damage, 0f, Main.myPlayer, 0f, 5, 50);
                        SoundStyle slash = new("CalamityMod/Sounds/Item/MurasamaBigSwing");
                        SoundEngine.PlaySound(slash with { Volume = 0.55f, Pitch = -0.3f - BigAttackLimit * 0.1f }, NPC.Center);
                    }
                    else if (BigAttackLimit == 0 && Phase2)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SoundStyle charge = new("CalamityMod/Sounds/Item/ExobladeBeamSlash");
                            SoundEngine.PlaySound(charge with { Volume = 0.85f, Pitch = -0.5f }, NPC.Center);
                            NPC.velocity = firingVelocity * 90f;
                            dashAttackTimer = 30;
                            dashes = 0;
                        }
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(SupremeCalamitas.CatastropheSwing with { Volume = 0.5f, Pitch = (SlashingFromRight ? 0.2f : -0.2f) }, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), slashSpawnPosition, NPC.DirectionTo(Target.Center) * 8f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt(), 1);
                        for (int i = 0; i < 7; i++)
                        {
                            SparkParticle orb = new SparkParticle(NPC.Center + NPC.DirectionTo(Target.Center) * 10f, (NPC.DirectionTo(Target.Center) * 30f).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.4f, 1.1f), false, 40, Main.rand.NextFloat(0.75f, 2.25f), Color.Cyan);
                            GeneralParticleHandler.SpawnParticle(orb);
                        }
                    }

                    SlashingFromRight = !SlashingFromRight;
                    CurrentFrame = 0;
                    if (BigAttackLimit > 0)
                        BigAttackLimit--;
                    else
                    {
                        BigAttackTimer = 500;
                        AttackDelayTimer = 0;
                        BigAttackLimit = 7;
                        
                        targetSide = !targetSide;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = NPC.frame.Size() * 0.5f;
            int afterimageCount = 4;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageCount; i += 2)
                {
                    Color afterimageColor = NPC.GetAlpha(Color.Lerp(drawColor, Color.White, 0.5f)) * ((afterimageCount - i) / 15f);
                    Vector2 drawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                    spriteBatch.Draw(texture, drawPosition, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 mainDrawPosition = NPC.Center - screenPos;
            spriteBatch.Draw(texture, mainDrawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            texture = GlowTexture.Value;
            Color baseGlowmaskColor = Color.Lerp(Color.White, Color.Cyan, 0.35f);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageCount; i++)
                {
                    Color afterimageColor = Color.Lerp(baseGlowmaskColor, Color.White, 0.5f) * ((afterimageCount - i) / 15f);
                    Vector2 drawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                    spriteBatch.Draw(texture, drawPosition, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, mainDrawPosition, NPC.frame, baseGlowmaskColor, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<SupremeCatastropheTrophy>(), 10);

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int brimDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimDust].scale = 0.5f;
                        Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[brimDust2].noGravity = true;
                    Main.dust[brimDust2].velocity *= 5f;
                    brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust2].velocity *= 2f;
                }

                // Turn into dust on death.
                if (NPC.life <= 0)
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
                    {
                        NPC scal = Main.npc[CalamityGlobalNPC.SCal];
                        if (scal.ModNPC<SupremeCalamitas>().respawnBro == true && Main.masterMode && !broIsAlive)
                        {
                            for (int i = 0; i < 45; i++)
                            {
                                Vector2 vel = new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 2.5f);
                                Dust cataclysmdust = Dust.NewDustPerfect(NPC.Center + vel * 2, 279, vel);
                                cataclysmdust.noGravity = true;
                                cataclysmdust.scale = Main.rand.NextFloat(1.2f, 1.8f);
                                cataclysmdust.color = Color.Red;
                            }
                            Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 25);
                            GeneralParticleHandler.SpawnParticle(pulse);
                            Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 28);
                            GeneralParticleHandler.SpawnParticle(pulse2);

                            SoundStyle respawn = new("CalamityMod/Sounds/NPCKilled/RavagerLimbLoss3");
                            SoundEngine.PlaySound(respawn with { Volume = 0.9f, Pitch = 0.3f }, NPC.Center);

                            CalamityUtils.SpawnBossBetter(NPC.Center, ModContent.NPCType<SupremeCataclysm>(), null, MovingUp ? -1 : 1);
                            scal.ModNPC<SupremeCalamitas>().respawnBro = false;
                        }

                    }
                    DeathAshParticle.CreateAshesFromNPC(NPC);
                }
            }
        }
    }
}

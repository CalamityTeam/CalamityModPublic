using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;
using Terraria.Audio;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [AutoloadBossHead]
    public class SupremeCatastrophe : ModNPC
    {
        public int VerticalOffset = -375;
        public int CurrentFrame;
        public bool SlashingFromRight;
        public const int HorizontalOffset = 750;
        public const int SlashCounterLimit = 45;
        public const int DartBurstCounterLimit = 300;
        public Player Target => Main.player[NPC.target];
        public ref float SlashCounter => ref NPC.ai[1];
        public ref float DartBurstCounter => ref NPC.ai[2];
        public ref float ElapsedVerticalDistance => ref NPC.ai[3];
        public ref float AttackDelayTimer => ref NPC.localAI[0];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.3f,
                PortraitPositionYOverride = 56f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
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
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.DD2_OgreRoar;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
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
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

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

            float acceleration = 1.5f;

            // Reduce acceleration if target is holding a true melee weapon
            if (Main.player[NPC.target].HoldingTrueMeleeWeapon())
                acceleration *= 0.5f;

            int verticalSpeed = (int)Math.Round(MathHelper.Lerp(2f, 6.5f, 1f - totalLifeRatio));

            // Move down.
            if (ElapsedVerticalDistance < HorizontalOffset)
            {
                ElapsedVerticalDistance += verticalSpeed;
                VerticalOffset += verticalSpeed;
            }

            // Move up.
            else if (ElapsedVerticalDistance < HorizontalOffset * 2)
            {
                ElapsedVerticalDistance += verticalSpeed;
                VerticalOffset -= verticalSpeed;
            }

            // Reset the vertical distance once a single period has concluded.
            else
                ElapsedVerticalDistance = 0f;

            // Reset rotation to zero.
            NPC.rotation = 0f;

            // Set direction.
            NPC.spriteDirection = (Target.Center.X > NPC.Center.X).ToDirectionInt();

            // Hover to the side of the target.
            Vector2 idealVelocity = NPC.SafeDirectionTo(Target.Center + new Vector2(-HorizontalOffset, VerticalOffset)) * 60f;
            NPC.SimpleFlyMovement(idealVelocity, acceleration);

            // Have a small delay prior to shooting projectiles.
            if (AttackDelayTimer < 120f)
                AttackDelayTimer++;

            // Handle projectile shots.
            else
            {
                // Shoot sword slashes.
                float fireRate = BossRushEvent.BossRushActive ? 2f : MathHelper.Lerp(1f, 2.5f, 1f - totalLifeRatio);
                SlashCounter += fireRate;
                if (SlashCounter >= SlashCounterLimit)
                {
                    SlashCounter = 0f;
                    SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound, NPC.Center);

                    int type = ModContent.ProjectileType<SupremeCatastropheSlash>();
                    int damage = NPC.GetProjectileDamage(type);
					if (bossRush)
						damage /= 2;
                    Vector2 slashSpawnPosition = NPC.Center + Vector2.UnitX * 125f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), slashSpawnPosition, Vector2.UnitX * 4f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt());
                    SlashingFromRight = !SlashingFromRight;
                    CurrentFrame = 0;
                }

                // Shoot dart spreads.
                fireRate = BossRushEvent.BossRushActive ? 3f : MathHelper.Lerp(1f, 4f, 1f - totalLifeRatio);
                DartBurstCounter += fireRate;
                if (DartBurstCounter >= DartBurstCounterLimit)
                {
                    DartBurstCounter = 0f;
                    SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound, NPC.Center);

                    int type = Main.zenithWorld ? ModContent.ProjectileType<BrimstoneHellblast2>() : ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = NPC.GetProjectileDamage(type);
					if (bossRush)
						damage /= 2;
                    int totalProjectiles = bossRush ? 20 : death ? 16 : revenge ? 14 : expertMode ? 12 : 8;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    float velocity = Main.zenithWorld ? 5f : 7f;
                    Vector2 spinningPoint = new Vector2(0f, -velocity);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, type, damage, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }

                    for (int i = 0; i < 6; i++)
                        Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f);
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

            if (CalamityConfig.Instance.Afterimages)
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

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeCatastropheGlow").Value;
            Color baseGlowmaskColor = Color.Lerp(Color.White, Color.Cyan, 0.35f);

            if (CalamityConfig.Instance.Afterimages)
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
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                // Turn into dust on death.
                if (NPC.life <= 0)
                    DeathAshParticle.CreateAshesFromNPC(NPC);
            }
        }
    }
}

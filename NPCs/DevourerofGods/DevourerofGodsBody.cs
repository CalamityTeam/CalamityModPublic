using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [LongDistanceNetSync(SyncWith = typeof(DevourerofGodsHead))]
    public class DevourerofGodsBody : ModNPC
    {
        public static int phase2IconIndex;

        internal static void LoadHeadIcons()
        {
            string phase2IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyS_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        private const float LaserVelocityMultiplierMin = 0.5f;

        private const float LaserVelocityDistanceMultiplier = LaserVelocityMultiplierMin * 0.05f;

        private int invinceTime = 360;
        private bool setOpacity = false;
        private bool phase2Started = false;
        public int SegmentIndex;

        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> Phase2Texture;
        public static Asset<Texture2D> Phase2Texture_Glow;
        public static Asset<Texture2D> Phase2Texture_Glow2;

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DevourerofGodsHead.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            if (!Main.dedServ)
            {
                Texture_Glow = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "S", AssetRequestMode.AsyncLoad);
                Phase2Texture_Glow = ModContent.Request<Texture2D>(Texture + "SGlow", AssetRequestMode.AsyncLoad);
                Phase2Texture_Glow2 = ModContent.Request<Texture2D>(Texture + "SGlow2", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 56;
            NPC.height = 56;
            NPC.defense = 70;
            CalamityGlobalNPC global = NPC.Calamity();
            if (!Main.zenithWorld)
            {
                global.DR = 0.925f;
                global.unbreakableDR = true;
                NPC.chaseable = false;
            }
            NPC.LifeMaxNERB(887500, 1065000, 1500000); // Phase 1 is 355000, Phase 2 is 532500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.dontCountMe = true;

            if (Main.getGoodWorld)
                NPC.scale *= 1.5f;
        }

        public override void BossHeadSlot(ref int index)
        {
            NPC head = CalamityGlobalNPC.DoGHead >= 0 ? Main.npc[CalamityGlobalNPC.DoGHead] : null;
            DevourerofGodsHead modNPC = head?.ModNPC<DevourerofGodsHead>() ?? null;
            if (head is null || modNPC.AwaitingPhase2Teleport || !modNPC.Phase2Started)
            {
                index = -1;
                return;
            }
            index = phase2IconIndex;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            NPC head = CalamityGlobalNPC.DoGHead >= 0 ? Main.npc[CalamityGlobalNPC.DoGHead] : null;
            DevourerofGodsHead modNPC = head?.ModNPC<DevourerofGodsHead>() ?? null;
            if (head is null || modNPC.AwaitingPhase2Teleport || !modNPC.Phase2Started)
                return;

            rotation = NPC.rotation;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2Started);
            writer.Write(invinceTime);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(setOpacity);
            writer.Write(NPC.Opacity);
            writer.Write(SegmentIndex);
            writer.Write(NPC.frame.X);
            writer.Write(NPC.frame.Y);
            writer.Write(NPC.frame.Width);
            writer.Write(NPC.frame.Height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2Started = reader.ReadBoolean();
            invinceTime = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            setOpacity = reader.ReadBoolean();
            NPC.Opacity = reader.ReadSingle();
            SegmentIndex = reader.ReadInt32();
            Rectangle frame = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            if (frame.Width > 0 && frame.Height > 0)
                NPC.frame = frame;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            bool phase2 = lifeRatio < 0.6f;
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            if (phase2)
            {
                phase2Started = true;

                // Once before DoG spawns, set new size
                if (Main.npc[(int)NPC.ai[2]].localAI[2] == 60f)
                {
                    NPC.position = NPC.Center;
                    NPC.width = (int)(70 * NPC.scale);
                    NPC.height = (int)(70 * NPC.scale);
                    NPC.frame = new Rectangle(0, 0, 114, 88);
                    NPC.position -= NPC.Size * 0.5f;

                    NPC.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;
                }
            }

            if (invinceTime > 0)
            {
                invinceTime--;
                NPC.dontTakeDamage = true;
            }
            else
                NPC.dontTakeDamage = Main.npc[(int)NPC.ai[2]].dontTakeDamage;

            if (Main.npc[(int)NPC.ai[2]].dontTakeDamage)
                invinceTime = 240;

            // Target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>());
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            // Lasers
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!NPC.dontTakeDamage && NPC.Opacity >= 1f && invinceTime <= 0)
                {
                    if (phase2)
                    {
                        // Fire lasers from every 15th (20th in normal mode) body segment if not in laser wall phase
                        float laserWallPhaseGateValue = 720f;
                        if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[3] < laserWallPhaseGateValue - 180f)
                        {
                            NPC.localAI[0] += 1f;
                            float laserGateValue = bossRush ? 156f : death ? 180f : 192f;
                            if (Main.getGoodWorld)
                                laserGateValue *= 0.5f;

                            if (NPC.localAI[0] >= laserGateValue && NPC.ai[0] % (expertMode ? 15f : 20f) == 0f)
                            {
                                NPC.localAI[0] = 0f;
                                if (!AnyTeleportRifts())
                                {
                                    NPC.TargetClosest();
                                    SoundEngine.PlaySound(SoundID.Item12, player.Center);
                                    float maxProjectileVelocity = bossRush ? 24f : death ? 20f : revenge ? 18.25f : expertMode ? 17.5f : 16f;
                                    float minProjectileVelocity = maxProjectileVelocity * LaserVelocityMultiplierMin;
                                    float projectileVelocity = MathHelper.Clamp(Vector2.Distance(player.Center, NPC.Center) * LaserVelocityDistanceMultiplier, minProjectileVelocity, maxProjectileVelocity);
                                    Vector2 velocityVector = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                                    int type = ModContent.ProjectileType<DoGDeath>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocityVector, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Fire lasers from every 15th (20th in normal mode) body segment if not in laser barrage phase
                        float laserBarrageGateValue = bossRush ? 780f : death ? 900f : 960f;
                        float laserBarrageShootGateValue = bossRush ? 160f : 240f;
                        float laserBarragePhaseGateValue = laserBarrageGateValue - laserBarrageShootGateValue * 1.5f;
                        if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[1] < laserBarragePhaseGateValue)
                        {
                            NPC.localAI[0] += 1f;
                            if (NPC.localAI[0] >= laserBarrageGateValue * (Main.getGoodWorld ? 0.1f : 0.2f) && NPC.ai[0] % (expertMode ? 15f : 20f) == 0f)
                            {
                                NPC.TargetClosest();
                                SoundEngine.PlaySound(SoundID.Item12, player.Center);
                                NPC.localAI[0] = 0f;
                                float maxProjectileVelocity = bossRush ? 22f : death ? 18f : revenge ? 16.25f : expertMode ? 15.5f : 14f;
                                float minProjectileVelocity = maxProjectileVelocity * LaserVelocityMultiplierMin;
                                float projectileVelocity = MathHelper.Clamp(Vector2.Distance(player.Center, NPC.Center) * LaserVelocityDistanceMultiplier, minProjectileVelocity, maxProjectileVelocity);
                                Vector2 velocityVector = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                                int type = ModContent.ProjectileType<DoGDeath>();
                                int damage = NPC.GetProjectileDamage(type);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocityVector, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }

            if (Main.npc[(int)NPC.ai[1]].Opacity >= 0.5f && (!setOpacity || (Main.npc[(int)NPC.ai[2]].localAI[2] <= 60f && Main.npc[(int)NPC.ai[2]].localAI[2] > 0f)))
            {
                NPC.Opacity += 0.165f;
                if (NPC.Opacity >= 1f && invinceTime <= 0)
                {
                    setOpacity = true;
                    NPC.Opacity = 1f;
                }
            }
            else
            {
                if (Main.npc[(int)NPC.ai[2]].ModNPC<DevourerofGodsHead>()?.AttemptingToEnterPortal ?? false)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile portal = Main.projectile[Main.npc[(int)NPC.ai[2]].ModNPC<DevourerofGodsHead>().PortalIndex];
                        float newOpacity = 1f - Utils.GetLerpValue(270f, 100f, NPC.Distance(portal.Center), true);
                        if (newOpacity > 0f && NPC.Opacity > newOpacity)
                        {
                            NPC.Opacity = newOpacity;

                            // Create dust at the portal position.
                            if (Vector2.Dot((NPC.rotation - MathHelper.PiOver2).ToRotationVector2(), Main.npc[(int)NPC.ai[2]].velocity) > 0f)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust cosmicMagic = Dust.NewDustPerfect(portal.Center, Main.rand.NextBool() ? 180 : 173);
                                    cosmicMagic.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 8f);
                                    cosmicMagic.scale *= Main.rand.NextFloat(1f, 1.8f);
                                    cosmicMagic.noGravity = true;
                                }
                            }

                            if (NPC.Opacity < 0.2f)
                                NPC.Opacity = 0f;

                            NPC.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (NPC.netSpam >= 10)
                                NPC.netSpam = 9;
                        }
                    }
                }
                else
                    NPC.Opacity = Main.npc[(int)NPC.ai[2]].Opacity;
            }

            NPC aheadSegment = Main.npc[(int)NPC.ai[1]];
            Vector2 directionToNextSegment = aheadSegment.Center - NPC.Center;
            if (aheadSegment.rotation != NPC.rotation)
            {
                directionToNextSegment = directionToNextSegment.RotatedBy(MathHelper.WrapAngle(aheadSegment.rotation - NPC.rotation) * 0.08f);
                directionToNextSegment = directionToNextSegment.MoveTowards((aheadSegment.rotation - NPC.rotation).ToRotationVector2(), 1f);
            }

            // Decide segment offset stuff.
            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.Center = aheadSegment.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.scale * NPC.width;
            NPC.spriteDirection = (directionToNextSegment.X > 0).ToDirectionInt();

            // Velocity variables
            float segmentVelocity = bossRush ? 19f : death ? 17.5f : 16f;
            if (expertMode)
                segmentVelocity += 4f * (1f - lifeRatio);
            if (Main.getGoodWorld)
                segmentVelocity *= 1.1f;

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = segmentVelocity * 0.25f;
            float minimalDamageVelocity = segmentVelocity * 0.5f;
            float bodyAndTailVelocity = (NPC.position - NPC.oldPosition).Length();
            if (bodyAndTailVelocity <= minimalContactDamageVelocity)
            {
                NPC.damage = 0;
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp(0f, NPC.defDamage, velocityDamageScalar);
            }
        }

        private bool AnyTeleportRifts()
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<DoGTeleportRift>())
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.realLife < 0 || NPC.realLife >= Main.maxNPCs || Main.npc[NPC.realLife] is null)
                return true;
            if (Main.npc[NPC.realLife].type != ModContent.NPCType<DevourerofGodsHead>())
                return true;

            float disintegrationFactor = Main.npc[NPC.realLife].ModNPC<DevourerofGodsHead>().DeathAnimationTimer / 640f;
            if (disintegrationFactor > 0f)
            {
                spriteBatch.EnterShaderRegion();
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseOpacity(disintegrationFactor);
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseSaturation(NPC.whoAmI);
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseImage1("Images/Misc/Perlin");
                GameShaders.Misc["CalamityMod:DoGDisintegration"].Apply();
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool useOtherTextures = phase2Started && Main.npc[(int)NPC.ai[2]].localAI[2] <= 60f;
            Texture2D texture2D15 = useOtherTextures ? Phase2Texture.Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            if (!NPC.dontTakeDamage)
            {
                if (useOtherTextures)
                {
                    texture2D15 = Phase2Texture_Glow.Value;
                    Color glowmaskColor = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

                    spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }

                texture2D15 = useOtherTextures ? Phase2Texture_Glow2.Value : Texture_Glow.Value;
                Color glowmaskColor2 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

                spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor2, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

            if (disintegrationFactor > 0f)
                spriteBatch.ExitShaderRegion();

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= (phase2Started ? 55f : 40f) * NPC.scale && NPC.Opacity >= 1f && invinceTime <= 0;
        }

        // This will always put the boss to 1 health before dying, which makes external checks work.
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers) => modifiers.SetMaxDamage(NPC.life - 1);

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // viable???, done here since it's conditional
            if (Main.zenithWorld && projectile.type == ModContent.ProjectileType<LaceratorYoyo>())
            {
                modifiers.SourceDamage *= 40f;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            NPC.active = true;

            NPC.netUpdate = true;

            // Prevent netUpdate from being blocked by the spam counter.
            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;

            if (NPC.realLife >= 0)
            {
                NPC Head = Main.npc[NPC.realLife];
                if (Head.type != ModContent.NPCType<DevourerofGodsHead>())
                    return false;

                Head.ModNPC<DevourerofGodsHead>().Dying = true;
                Head.dontTakeDamage = true;

                Head.netUpdate = true;

                // Prevent netUpdate from being blocked by the spam counter.
                if (Head.netSpam >= 10)
                    Head.netSpam = 9;
            }
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("DoGS6").Type, NPC.scale);
                }
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(100 * NPC.scale);
                NPC.height = (int)(100 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 10; i++)
                {
                    int cosmiliteDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[cosmiliteDust].scale = 0.5f;
                        Main.dust[cosmiliteDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[cosmiliteDust2].noGravity = true;
                    Main.dust[cosmiliteDust2].velocity *= 5f;
                    cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust2].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 160, true);
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 480, true);
            }
        }
    }
}

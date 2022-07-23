using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DevourerofGodsBody : ModNPC
    {
        public static int phase2IconIndex;

        internal static void LoadHeadIcons()
        {
            string phase2IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyS_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        private int invinceTime = 360;
        private bool setOpacity = false;
        private bool phase2Started = false;
        public int SegmentIndex;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("The Devourer of Gods");
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
            global.DR = 0.925f;
            global.unbreakableDR = true;
            NPC.LifeMaxNERB(888750, 1066500, 1500000); // Phase 1 is 371250, Phase 2 is 517500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.chaseable = false;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.boss = true;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP1") ?? MusicID.Boss3;
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

            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.6f;
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            if (phase2)
            {
                phase2Started = true;

                // Play music after the transiton BS
                if (Main.npc[(int)NPC.ai[2]].localAI[2] == 530f)
                    Music = CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP2") ?? MusicID.LunarBoss;

                // Once before DoG spawns, set new size
                if (Main.npc[(int)NPC.ai[2]].localAI[2] == 60f)
                {
                    NPC.position = NPC.Center;
                    NPC.width = (int)(70 * NPC.scale);
                    NPC.height = (int)(70 * NPC.scale);
                    NPC.frame = new Rectangle(0, 0, 114, 88);
                    NPC.position -= NPC.Size * 0.5f;
                    NPC.netUpdate = true;
                }
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

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
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<DevourerofGodsHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
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
                        // Fire lasers from every 10th (20th in normal mode) body segment if not in laser wall phase
                        float laserWallPhaseGateValue = 720f;
                        if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[3] < laserWallPhaseGateValue - 180f)
                        {
                            NPC.localAI[0] += 1f;
                            float laserGateValue = bossRush ? 156f : death ? 180f : 192f;
                            if (Main.getGoodWorld)
                                laserGateValue *= 0.5f;

                            if (NPC.localAI[0] >= laserGateValue && NPC.ai[0] % (expertMode ? 10f : 20f) == 0f)
                            {
                                NPC.localAI[0] = 0f;
                                if (!AnyTeleportRifts())
                                {
                                    NPC.TargetClosest();
                                    SoundEngine.PlaySound(SoundID.Item12, player.position);
                                    float maxProjectileVelocity = bossRush ? 8f : death ? 7.5f : revenge ? 7.25f : expertMode ? 7f : 6.5f;
                                    float minProjectileVelocity = maxProjectileVelocity * 0.25f;
                                    float projectileVelocity = MathHelper.Clamp(Vector2.Distance(player.Center, NPC.Center) * 0.01f, minProjectileVelocity, maxProjectileVelocity);
                                    Vector2 velocityVector = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                                    int type = ModContent.ProjectileType<DoGDeath>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocityVector, type, damage, 0f, Main.myPlayer);
                                    Main.projectile[proj].timeLeft = 1800;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Fire lasers from every 10th (20th in normal mode) body segment if not in laser barrage phase
                        float laserBarrageGateValue = bossRush ? 780f : death ? 900f : 960f;
                        float laserBarrageShootGateValue = bossRush ? 160f : 240f;
                        float laserBarragePhaseGateValue = laserBarrageGateValue - laserBarrageShootGateValue * 1.5f;
                        if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[1] < laserBarragePhaseGateValue)
                        {
                            NPC.localAI[0] += 1f;
                            if (NPC.localAI[0] >= laserBarrageGateValue * (Main.getGoodWorld ? 0.1f : 0.2f) && NPC.ai[0] % (expertMode ? 10f : 20f) == 0f)
                            {
                                NPC.TargetClosest();
                                SoundEngine.PlaySound(SoundID.Item12, player.position);
                                NPC.localAI[0] = 0f;
                                float maxProjectileVelocity = bossRush ? 7.5f : death ? 7f : revenge ? 6.75f : expertMode ? 6.5f : 6f;
                                float minProjectileVelocity = maxProjectileVelocity * 0.25f;
                                float projectileVelocity = MathHelper.Clamp(Vector2.Distance(player.Center, NPC.Center) * 0.01f, minProjectileVelocity, maxProjectileVelocity);
                                Vector2 velocityVector = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                                int type = ModContent.ProjectileType<DoGDeath>();
                                int damage = NPC.GetProjectileDamage(type);
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocityVector, type, damage, 0f, Main.myPlayer);
                                Main.projectile[proj].timeLeft = 1800;
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
                        }
                    }
                }
                else
                    NPC.Opacity = Main.npc[(int)NPC.ai[2]].Opacity;
            }

            Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num191 = player.position.X + (player.width / 2);
            float num192 = player.position.Y + (player.height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }

                NPC.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = NPC.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;

                if (num191 < 0f)
                    NPC.spriteDirection = -1;
                else if (num191 > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        private bool AnyTeleportRifts()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<DoGTeleportRift>())
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
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
            Texture2D texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyS").Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            if (!NPC.dontTakeDamage)
            {
                if (useOtherTextures)
                {
                    texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodySGlow").Value;
                    Color color36 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

                    spriteBatch.Draw(texture2D15, vector43, NPC.frame, color36, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }

                texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodySGlow2").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyGlow").Value;
                Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
            }

            if (disintegrationFactor > 0f)
                spriteBatch.ExitShaderRegion();

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= (phase2Started ? 55f : 40f) * NPC.scale && NPC.Opacity >= 1f && invinceTime <= 0;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if ((damage * (crit ? 2D : 1D)) >= NPC.life || (NPC.realLife >= 0 && Main.npc[NPC.realLife].ModNPC<DevourerofGodsHead>().Dying))
            {
                if (NPC.realLife >= 0)
                {
                    damage = 0D;
                    NPC.dontTakeDamage = true;
                    Main.npc[NPC.realLife].dontTakeDamage = true;
                    Main.npc[NPC.realLife].life = 1;

                    if (!Main.npc[NPC.realLife].ModNPC<DevourerofGodsHead>().Dying)
                    {
                        Main.npc[NPC.realLife].ModNPC<DevourerofGodsHead>().Dying = true;
                        Main.npc[NPC.realLife].dontTakeDamage = true;
                        Main.npc[NPC.realLife].active = true;
                        Main.npc[NPC.realLife].netUpdate = true;
                    }
                }
                return false;
            }
            return true;
        }

        public override bool CheckDead()
        {
            if (NPC.realLife >= 0)
                Main.npc[NPC.realLife].checkDead();

            return base.CheckDead();
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
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
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 240, true);
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 480, true);
        }
    }
}

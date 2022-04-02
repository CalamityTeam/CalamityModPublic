using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
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
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.GetNPCDamage();
            npc.npcSlots = 5f;
            npc.width = 56;
            npc.height = 56;
            npc.defense = 70;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.925f;
            global.unbreakableDR = true;
            npc.LifeMaxNERB(888750, 1066500, 1500000); // Phase 1 is 371250, Phase 2 is 517500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.Opacity = 0f;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.chaseable = false;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            npc.boss = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP1") ?? MusicID.Boss3;
            npc.dontCountMe = true;
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
            
            rotation = npc.rotation;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2Started);
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
            writer.Write(setOpacity);
            writer.Write(npc.Opacity);
            writer.Write(SegmentIndex);
            writer.Write(npc.frame.X);
            writer.Write(npc.frame.Y);
            writer.Write(npc.frame.Width);
            writer.Write(npc.frame.Height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2Started = reader.ReadBoolean();
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
            setOpacity = reader.ReadBoolean();
            npc.Opacity = reader.ReadSingle();
            SegmentIndex = reader.ReadInt32();
            Rectangle frame = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            if (frame.Width > 0 && frame.Height > 0)
                npc.frame = frame;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            if (npc.life > Main.npc[(int)npc.ai[1]].life)
                npc.life = Main.npc[(int)npc.ai[1]].life;

            if (npc.life / (float)npc.lifeMax < 0.6f)
            {
                phase2Started = true;

                // Play music after the transiton BS
                if (CalamityWorld.DoGSecondStageCountdown == 530)
                    music = CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP2") ?? MusicID.LunarBoss;

                // Once before DoG spawns, set new size
                if (CalamityWorld.DoGSecondStageCountdown == 60)
                {
                    npc.position = npc.Center;
                    npc.width = 70;
                    npc.height = 70;
                    npc.frame = new Rectangle(0, 0, 114, 88);
                    npc.position -= npc.Size * 0.5f;
                    npc.netUpdate = true;
                }
            }

            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            if (invinceTime > 0)
            {
                invinceTime--;
                npc.dontTakeDamage = true;
            }
            else
                npc.dontTakeDamage = Main.npc[(int)npc.ai[2]].dontTakeDamage;

            if (Main.npc[(int)npc.ai[2]].dontTakeDamage)
                invinceTime = 240;

            // Target
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

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
                if (npc.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)npc.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
                npc.active = false;
            }

            if (Main.npc[(int)npc.ai[1]].Opacity >= 0.5f && (!setOpacity || (CalamityWorld.DoGSecondStageCountdown <= 60 && CalamityWorld.DoGSecondStageCountdown > 0)))
            {
                npc.Opacity += 0.165f;
                if (npc.Opacity >= 1f && invinceTime <= 0)
                {
                    setOpacity = true;
                    npc.Opacity = 1f;
                }
            }
            else
            {
                if (Main.npc[(int)npc.ai[2]].ModNPC<DevourerofGodsHead>()?.AttemptingToEnterPortal ?? false)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient) 
                    {
                        Projectile portal = Main.projectile[Main.npc[(int)npc.ai[2]].ModNPC<DevourerofGodsHead>().PortalIndex];
                        float newOpacity = 1f - Utils.InverseLerp(270f, 100f, npc.Distance(portal.Center), true);
                        if (newOpacity > 0f && npc.Opacity > newOpacity)
                        {
                            npc.Opacity = newOpacity;

                            // Create dust at the portal position.
                            if (Vector2.Dot((npc.rotation - MathHelper.PiOver2).ToRotationVector2(), Main.npc[(int)npc.ai[2]].velocity) > 0f)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust cosmicMagic = Dust.NewDustPerfect(portal.Center, Main.rand.NextBool() ? 180 : 173);
                                    cosmicMagic.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 8f);
                                    cosmicMagic.scale *= Main.rand.NextFloat(1f, 1.8f);
                                    cosmicMagic.noGravity = true;
                                }
                            }

                            if (npc.Opacity < 0.2f)
                                npc.Opacity = 0f;

                            npc.netUpdate = true;
                        }
                    }
                }
                else
                    npc.Opacity = Main.npc[(int)npc.ai[2]].Opacity;
            }

            Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num191 = player.position.X + (player.width / 2);
            float num192 = player.position.Y + (player.height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }

                npc.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = npc.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;

                if (num191 < 0f)
                    npc.spriteDirection = -1;
                else if (num191 > 0f)
                    npc.spriteDirection = 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float disintegrationFactor = Main.npc[npc.realLife].ModNPC<DevourerofGodsHead>().DeathAnimationTimer / 640f;
            if (disintegrationFactor > 0f)
            {
                spriteBatch.EnterShaderRegion();
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseOpacity(disintegrationFactor);
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseSaturation(npc.whoAmI);
                GameShaders.Misc["CalamityMod:DoGDisintegration"].UseImage("Images/Misc/Perlin");
                GameShaders.Misc["CalamityMod:DoGDisintegration"].Apply();
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool useOtherTextures = phase2Started && CalamityWorld.DoGSecondStageCountdown <= 60;
            Texture2D texture2D15 = useOtherTextures ? ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyS") : Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            if (!npc.dontTakeDamage)
            {
                if (useOtherTextures)
                {
                    texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodySGlow");
                    Color color36 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

                    spriteBatch.Draw(texture2D15, vector43, npc.frame, color36, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }

                texture2D15 = useOtherTextures ? ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodySGlow2") : ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsBodyGlow");
                Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

                spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
            }

            if (disintegrationFactor > 0f)
                spriteBatch.ExitShaderRegion();

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= (phase2Started ? 55f : 40f) && npc.Opacity >= 1f && invinceTime <= 0;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if ((damage * (crit ? 2D : 1D)) >= npc.life || (npc.realLife >= 0 && Main.npc[npc.realLife].ModNPC<DevourerofGodsHead>().Dying))
            {
                if (npc.realLife >= 0)
                {
                    damage = 0D;
                    npc.dontTakeDamage = true;
                    Main.npc[npc.realLife].dontTakeDamage = true;
                    Main.npc[npc.realLife].life = 1;

                    if (!Main.npc[npc.realLife].ModNPC<DevourerofGodsHead>().Dying)
                    {
                        Main.npc[npc.realLife].ModNPC<DevourerofGodsHead>().Dying = true;
                        Main.npc[npc.realLife].dontTakeDamage = true;
                        Main.npc[npc.realLife].active = true;
                        Main.npc[npc.realLife].netUpdate = true;
                    }
                }
                return false;
            }
            return true;
        }

        public override bool CheckDead()
        {
            if (npc.realLife >= 0)
                Main.npc[npc.realLife].checkDead();
            return base.CheckDead();
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                float randomSpread = Main.rand.Next(-100, 100) / 100;
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/DoGS6"), 1f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 20; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 240, true);
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 300, true);
        }
    }
}

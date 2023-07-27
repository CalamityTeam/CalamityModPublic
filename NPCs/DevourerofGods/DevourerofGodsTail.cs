using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DevourerofGodsTail : ModNPC
    {
        public static int phase1IconIndex;
        public static int phase2IconIndex;

        internal static void LoadHeadIcons()
        {
            string phase1IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsTail_Head_Boss";
            string phase2IconPath = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailS_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
            phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        private int invinceTime = 720;
        private bool setOpacity = false;
        private bool phase2Started = false;
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DevourerofGodsHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        internal void setInvulTime(int time)
        {
            invinceTime = time;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 66;
            NPC.height = 66;
            NPC.defense = 50;
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
            NPC.canGhostHeal = false;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.takenDamageMultiplier = 1.25f;
            NPC.dontCountMe = true;

            if (Main.getGoodWorld)
                NPC.scale *= 1.5f;
        }

        public override void BossHeadSlot(ref int index)
        {
            NPC head = CalamityGlobalNPC.DoGHead >= 0 ? Main.npc[CalamityGlobalNPC.DoGHead] : null;
            DevourerofGodsHead modNPC = head?.ModNPC<DevourerofGodsHead>() ?? null;

            index = -1;
            if (head is null)
                return;

            if (!modNPC.Phase2Started)
            {
                index = phase1IconIndex;
                return;
            }

            if (!modNPC.AwaitingPhase2Teleport)
                index = phase2IconIndex;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            NPC head = CalamityGlobalNPC.DoGHead >= 0 ? Main.npc[CalamityGlobalNPC.DoGHead] : null;
            DevourerofGodsHead modNPC = head?.ModNPC<DevourerofGodsHead>() ?? null;
            if (head != null || modNPC.AwaitingPhase2Teleport || !modNPC.Phase2Started)
                return;

            rotation = NPC.rotation;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2Started);
            writer.Write(invinceTime);
            writer.Write(setOpacity);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.Opacity);
            writer.Write(NPC.frame.X);
            writer.Write(NPC.frame.Y);
            writer.Write(NPC.frame.Width);
            writer.Write(NPC.frame.Height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2Started = reader.ReadBoolean();
            invinceTime = reader.ReadInt32();
            setOpacity = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.Opacity = reader.ReadSingle();
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

            if (NPC.life / (float)NPC.lifeMax < 0.6f)
            {
                phase2Started = true;

                // Once before DoG spawns, set new size
                if (Main.npc[(int)NPC.ai[2]].localAI[2] == 60f)
                {
                    NPC.position = NPC.Center;
                    NPC.width = (int)(80 * NPC.scale);
                    NPC.height = (int)(140 * NPC.scale);
                    NPC.frame = new Rectangle(0, 0, 86, 148);
                    NPC.position -= NPC.Size * 0.5f;
                    NPC.netUpdate = true;
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

            float distanceFromTarget = Vector2.Distance(player.Center, NPC.Center);
            float takeLessDamageDistance = 1600f;
            if (distanceFromTarget > takeLessDamageDistance)
            {
                float damageTakenScalar = MathHelper.Clamp(1f - ((distanceFromTarget - takeLessDamageDistance) / takeLessDamageDistance), 0f, 1f);
                NPC.takenDamageMultiplier = MathHelper.Lerp(1f, 1.25f, damageTakenScalar);
            }
            else
                NPC.takenDamageMultiplier = 1.25f;

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
            float num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
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
                NPC.rotation = (float)System.Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
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
            Texture2D texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailS").Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            if (!NPC.dontTakeDamage)
            {
                texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailSGlow").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailGlow").Value;
                Color color37 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                texture2D15 = useOtherTextures ? ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailSGlow2").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsTailGlow2").Value;
                color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
            }

            if (disintegrationFactor > 0f)
                spriteBatch.ExitShaderRegion();

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

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

            return minDist <= (phase2Started ? 70f : 35f) * NPC.scale && NPC.Opacity >= 1f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 8;
                float extrapitch = Main.zenithWorld ? 0.3f : 0f;
                SoundEngine.PlaySound(CommonCalamitySounds.OtherwordlyHitSound with { Pitch = CommonCalamitySounds.OtherwordlyHitSound.Pitch + extrapitch }, NPC.Center);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoGS4").Type, NPC.scale);
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

            if (NPC.realLife >= 0)
            {
                NPC Head = Main.npc[NPC.realLife];
                if (Head.type != ModContent.NPCType<DevourerofGodsHead>())
                    return false;

                Head.ModNPC<DevourerofGodsHead>().Dying = true;
                Head.dontTakeDamage = true;
                Head.netUpdate = true;
            }
            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180, true);
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 360, true);
            }
        }
    }
}

using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;
using Steamworks;
using CalamityMod.Items.Weapons.Summon;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SoulSeekerSupreme : ModNPC
    {
        private int timer = 0;

        private bool start = true;
        public Player Target => Main.player[NPC.target];

        public Vector2 EyePosition => NPC.Center + new Vector2(NPC.spriteDirection == -1 ? 40f : -36f, 16f);
        public ref float RotationalDegreeOffset => ref NPC.ai[1];

        public static NPC SCal => Main.npc[CalamityGlobalNPC.SCal];

        public const float NormalDR = 0.25f;

        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitPositionXOverride = -5f,
                SpriteDirection = 1
            };
            value.Position.X += 12f;
            value.Position.Y -= 4f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 40;
            NPC.height = 40;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.damage = 50;
            NPC.defense = 60;
            NPC.DR_NERD(NormalDR);
            NPC.lifeMax = 28000;
            double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<SupremeCalamitas>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SoulSeekerSupreme")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 5 == 4)
                NPC.frame.Y += frameHeight;
            if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            // Die if SCal is no longer present.
            if (CalamityGlobalNPC.SCal < 0 || !SCal.active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (start)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                RotationalDegreeOffset = NPC.ai[0];
                start = false;
            }

            // Increase DR if the target leaves SCal's arena.
            NPC.Calamity().DR = NormalDR;
            if (SCal.ModNPC<SupremeCalamitas>().IsTargetOutsideOfArena)
                NPC.Calamity().DR = SupremeCalamitas.enragedDR;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Target.dead || !Target.active)
                NPC.TargetClosest();

            // Target another player if the current player target is too far away
            if (!NPC.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance200Tiles))
                NPC.TargetClosest();

            NPC.spriteDirection = (Target.Center.X < NPC.Center.X).ToDirectionInt();

            timer++;
            int shootRate = BossRushEvent.BossRushActive ? 120 : 180;
            if (timer > shootRate)
            {
                foreach (NPC seeker in Main.ActiveNPCs)
                {
                    if (seeker.type == NPC.type)
                    {
                        if (seeker == NPC)
                            SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound, SCal.Center);

                        break;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float targetDist = Vector2.Distance(Target.Center, NPC.Center);
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = NPC.GetProjectileDamage(type);
					if (BossRushEvent.BossRushActive)
						damage /= 2;

                    float velocity = 5f;
                    float projectileVelocityToPass = velocity * 3f;
                    Vector2 shootVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;
                    if (targetDist <= 160 || targetDist >= 1952)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                            failShotDust.noGravity = true;
                            failShotDust.velocity = new Vector2(3, 3).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                            failShotDust.scale = Main.rand.NextFloat(1.3f, 2.4f);
                        }
                    }
                    else
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootVelocity * 0.5f, type, damage, 1f, Main.myPlayer, 0f, 3f, projectileVelocityToPass);
                        for (int i = 0; i < 5; i++)
                        {
                            Dust ShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                            ShotDust.noGravity = true;
                            ShotDust.velocity = shootVelocity.RotatedByRandom(1f) * Main.rand.NextFloat(2.2f, 3.8f);
                            ShotDust.scale = Main.rand.NextFloat(1.8f, 2.1f);
                        }
                    }
                }

                timer = 0;

                NPC.netUpdate = true;
            }
            if (timer == shootRate - 35)
            {
                Particle pulse = new StaticPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(2f, 2f), 0, 0.03f, 0.005f, 8);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new StaticPulseRing(NPC.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(2f, 2f), 0, 0.025f, 0.005f, 8);
                GeneralParticleHandler.SpawnParticle(pulse2);
            }
            if (timer >= shootRate - 35)
            {
                Vector2 shootVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 5f;
                GlowOrbParticle spark2 = new GlowOrbParticle(NPC.Center + shootVelocity * 7, (shootVelocity * Main.rand.NextFloat(0.6f, 1.1f)) * 1.8f + NPC.velocity * 0.5f, false, 15, Main.rand.NextFloat(0.7f, 0.75f), Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.3f) : Color.Red);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            float distanceFromSCal = Main.getGoodWorld ? 300f : 225f;
            NPC.position = SCal.Center - MathHelper.ToRadians(RotationalDegreeOffset).ToRotationVector2() * distanceFromSCal - NPC.Size * 0.5f;
            RotationalDegreeOffset += 0.5f;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 5; i++)
                {
                    int brimDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimDust].scale = 0.5f;
                        Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[brimDust2].noGravity = true;
                    Main.dust[brimDust2].velocity *= 5f;
                    brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust2].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i <= 5; i++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"SupremeSoulSeeker_Gore{i}").Type, NPC.scale);
                }
            }
        }

        public override bool CheckActive() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            int afterimageAmt = 2;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color redLerp = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color redAfterimageColor = redLerp;
                    redAfterimageColor = Color.Lerp(redAfterimageColor, Color.White, 0.5f);
                    redAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 redAfterimagePos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    redAfterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    redAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, redAfterimagePos, NPC.frame, redAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, redLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }
    }
}

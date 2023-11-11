using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Polterghast
{
    [AutoloadBossHead]
    public class PolterPhantom : ModNPC
    {
        private int despawnTimer = 600;
        private bool reachedChargingPoint = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 90;
            NPC.height = 120;
            NPC.defense = 45;
            NPC.DR_NERD(0.1f);
            NPC.LifeMaxNERB(62500, 75000, 60000);
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            bool polterHasTarget = CalamityGlobalNPC.ghostBoss.WithinBounds(Main.maxNPCs) && Main.npc[CalamityGlobalNPC.ghostBoss].active && Main.npc[CalamityGlobalNPC.ghostBoss].HasValidTarget;
            if (polterHasTarget && NPC.Calamity().newAI[3] == 0f)
                rotation = (Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
            else
                rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawnTimer);
            writer.Write(reachedChargingPoint);
            CalamityGlobalNPC cgn = NPC.Calamity();
            writer.Write(cgn.newAI[0]);
            writer.Write(cgn.newAI[1]);
            writer.Write(cgn.newAI[2]);
            writer.Write(cgn.newAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawnTimer = reader.ReadInt32();
            reachedChargingPoint = reader.ReadBoolean();
            CalamityGlobalNPC cgn = NPC.Calamity();
            cgn.newAI[0] = reader.ReadSingle();
            cgn.newAI[1] = reader.ReadSingle();
            cgn.newAI[2] = reader.ReadSingle();
            cgn.newAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC.ghostBossClone = NPC.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.5f, 0.25f, 0.75f);

            Player player = Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target];

            // Percent life remaining, Polter
            float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

            Vector2 vector = NPC.Center;

            float chargePhaseGateValue = 480f;
            if (Main.getGoodWorld)
                chargePhaseGateValue *= 0.5f;

            float colorChangeTime = 180f;
            float changeColorGateValue = chargePhaseGateValue - colorChangeTime;

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];
            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= chargePhaseGateValue - 60f || NPC.Calamity().newAI[3] == 1f;
            float chargeVelocity = 24f;
            float chargeAcceleration = 0.6f;
            float chargeDistance = 480f;

            bool speedBoost = false;
            bool despawnBoost = false;

            if (NPC.timeLeft < 1500)
                NPC.timeLeft = 1500;

            float velocity = 3f;
            float acceleration = 0.03f;
            if (!player.ZoneDungeon && !bossRush && player.position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                {
                    despawnBoost = true;
                    NPC.ai[1] = 0f;
                    NPC.Calamity().newAI[0] = 0f;
                    NPC.Calamity().newAI[1] = 0f;
                    NPC.Calamity().newAI[2] = 0f;
                    NPC.Calamity().newAI[3] = 0f;
                }

                speedBoost = true;
                velocity += 8f;
                acceleration = 0.15f;
            }
            else
                despawnTimer++;

            if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] < changeColorGateValue)
            {
                velocity = 21f;
                acceleration = 0.13f;
            }

            if (expertMode)
            {
                chargeVelocity += revenge ? 4f : 2f;
                velocity += revenge ? 5f : 3.5f;
                acceleration += revenge ? 0.035f : 0.025f;
            }

            // Predictiveness
            Vector2 predictionVector = chargePhase && bossRush ? player.velocity * 20f : Vector2.Zero;
            Vector2 lookAt = player.Center + predictionVector;
            Vector2 rotationVector = lookAt - vector;

            // Rotation
            if (NPC.Calamity().newAI[3] == 0f)
            {
                float playerXDestination = player.Center.X + predictionVector.X - vector.X;
                float playerYDestination = player.Center.Y + predictionVector.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2(playerYDestination, playerXDestination) + MathHelper.PiOver2;
            }
            else
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            NPC.damage = NPC.defDamage;

            if (!chargePhase)
            {
                // Set this here to avoid despawn issues
                reachedChargingPoint = false;

                NPC.ai[0] = 0f;

                NPC.Opacity += 0.02f;
                if (NPC.Opacity > 0.8f)
                    NPC.Opacity = 0.8f;

                float movementLimitX = 0f;
                float movementLimitY = 0f;
                int numHooks = 4;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<PolterghastHook>())
                    {
                        movementLimitX += Main.npc[i].Center.X;
                        movementLimitY += Main.npc[i].Center.Y;
                    }
                }
                movementLimitX /= numHooks;
                movementLimitY /= numHooks;

                Vector2 movementLimitVector = new Vector2(movementLimitX, movementLimitY);
                float movementLimitedXDist = player.Center.X - movementLimitVector.X;
                float movementLimitedYDist = player.Center.Y - movementLimitVector.Y;

                if (despawnBoost)
                {
                    movementLimitedYDist *= -1f;
                    movementLimitedXDist *= -1f;
                    velocity += 8f;
                }

                float movementLimitedDistance = (float)Math.Sqrt(movementLimitedXDist * movementLimitedXDist + movementLimitedYDist * movementLimitedYDist);
                float maxDistanceFromHooks = expertMode ? 650f : 500f;
                if (speedBoost || bossRush)
                    maxDistanceFromHooks += 250f;
                if (death)
                    maxDistanceFromHooks += maxDistanceFromHooks * 0.1f * (1f - lifeRatio);

                // Increase speed based on nearby active tiles
                velocity *= tileEnrageMult;
                acceleration *= tileEnrageMult;

                if (death)
                {
                    velocity += velocity * 0.15f * (1f - lifeRatio);
                    acceleration += acceleration * 0.15f * (1f - lifeRatio);
                }

                if (movementLimitedDistance >= maxDistanceFromHooks)
                {
                    movementLimitedDistance = maxDistanceFromHooks / movementLimitedDistance;
                    movementLimitedXDist *= movementLimitedDistance;
                    movementLimitedYDist *= movementLimitedDistance;
                }

                movementLimitX += movementLimitedXDist;
                movementLimitY += movementLimitedYDist;
                movementLimitVector = vector;
                movementLimitedXDist = movementLimitX - movementLimitVector.X;
                movementLimitedYDist = movementLimitY - movementLimitVector.Y;
                movementLimitedDistance = (float)Math.Sqrt(movementLimitedXDist * movementLimitedXDist + movementLimitedYDist * movementLimitedYDist);

                if (movementLimitedDistance < velocity)
                {
                    movementLimitedXDist = NPC.velocity.X;
                    movementLimitedYDist = NPC.velocity.Y;
                }
                else
                {
                    movementLimitedDistance = velocity / movementLimitedDistance;
                    movementLimitedXDist *= movementLimitedDistance;
                    movementLimitedYDist *= movementLimitedDistance;
                }

                if (NPC.velocity.X < movementLimitedXDist)
                {
                    NPC.velocity.X += acceleration;
                    if (NPC.velocity.X < 0f && movementLimitedXDist > 0f)
                        NPC.velocity.X += acceleration * 2f;
                }
                else if (NPC.velocity.X > movementLimitedXDist)
                {
                    NPC.velocity.X -= acceleration;
                    if (NPC.velocity.X > 0f && movementLimitedXDist < 0f)
                        NPC.velocity.X -= acceleration * 2f;
                }
                if (NPC.velocity.Y < movementLimitedYDist)
                {
                    NPC.velocity.Y += acceleration;
                    if (NPC.velocity.Y < 0f && movementLimitedYDist > 0f)
                        NPC.velocity.Y += acceleration * 2f;
                }
                else if (NPC.velocity.Y > movementLimitedYDist)
                {
                    NPC.velocity.Y -= acceleration;
                    if (NPC.velocity.Y > 0f && movementLimitedYDist < 0f)
                        NPC.velocity.Y -= acceleration * 2f;
                }
            }
            else
            {
                // Charge
                if (NPC.Calamity().newAI[3] == 1f)
                {
                    reachedChargingPoint = false;

                    NPC.Opacity += 0.06f;
                    if (NPC.Opacity > 0.8f)
                        NPC.Opacity = 0.8f;

                    if (NPC.Calamity().newAI[1] == 0f)
                    {
                        NPC.velocity = Vector2.Normalize(rotationVector) * chargeVelocity;
                        NPC.Calamity().newAI[1] = 1f;
                    }
                    else
                    {
                        NPC.Calamity().newAI[2] += 1f;

                        // Slow down for a few frames
                        float totalChargeTime = chargeDistance * 4f / chargeVelocity;
                        float slowDownTime = chargeVelocity;
                        if (NPC.Calamity().newAI[2] >= totalChargeTime - slowDownTime)
                            NPC.velocity *= 0.9f;

                        // Reset and either go back to normal or charge again
                        if (NPC.Calamity().newAI[2] >= totalChargeTime)
                        {
                            NPC.Calamity().newAI[1] = 0f;
                            NPC.Calamity().newAI[2] = 0f;
                            NPC.Calamity().newAI[3] = 0f;
                            NPC.ai[0] = 0f;
                            NPC.ai[1] += 1f;

                            if (NPC.ai[1] >= 3f)
                            {
                                // Reset and return to normal movement
                                NPC.Calamity().newAI[0] = 0f;
                                NPC.ai[1] = 0f;
                            }
                        }
                    }
                }
                else
                {
                    // Random location choice
                    if (NPC.ai[0] == 0f)
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.ai[0] = Main.rand.Next(2) + 1;
                        NPC.netUpdate = true;
                    }

                    // Pick a charging location
                    // Set charge locations X
                    if (Main.npc[CalamityGlobalNPC.ghostBoss].Center.X >= player.Center.X)
                        NPC.Calamity().newAI[1] = NPC.ai[0] == 1f ? player.Center.X - chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[1];
                    else
                        NPC.Calamity().newAI[1] = NPC.ai[0] == 1f ? player.Center.X + chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[1];

                    // Set charge locations Y
                    if (Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y >= player.Center.Y)
                        NPC.Calamity().newAI[2] = NPC.ai[0] == 2f ? player.Center.Y - chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[2];
                    else
                        NPC.Calamity().newAI[2] = NPC.ai[0] == 2f ? player.Center.Y + chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[2];

                    // Do not deal damage during movement to avoid cheap bullshit hits
                    NPC.damage = 0;

                    // Charge location
                    Vector2 chargeVector = new Vector2(NPC.Calamity().newAI[1], NPC.Calamity().newAI[2]);
                    Vector2 chargeLocationVelocity = Vector2.Normalize(chargeVector - vector) * chargeVelocity;

                    // Line up a charge
                    float chargeDistanceGateValue = 32f;

                    if (Vector2.Distance(vector, chargeVector) <= chargeDistanceGateValue * 3f)
                    {
                        NPC.Opacity += 0.06f;
                        if (NPC.Opacity > 0.8f)
                            NPC.Opacity = 0.8f;
                    }
                    else
                    {
                        NPC.Opacity -= 0.06f;
                        if (NPC.Opacity < 0f)
                            NPC.Opacity = 0f;
                    }

                    if (Vector2.Distance(vector, chargeVector) <= chargeDistanceGateValue || reachedChargingPoint)
                    {
                        // Emit dust
                        if (!reachedChargingPoint)
                        {
                            SoundEngine.PlaySound(SoundID.Item125, NPC.Center);
                            for (int i = 0; i < 30; i++)
                            {
                                int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 5f;
                            }
                        }

                        reachedChargingPoint = true;
                        NPC.velocity = Vector2.Zero;
                        NPC.Center = chargeVector;
                    }
                    else
                    {
                        // Reduce velocity and acceleration to allow for smoother movement inside this loop
                        if (Vector2.Distance(vector, chargeVector) > 1200f)
                            NPC.velocity = chargeLocationVelocity;
                        else
                            NPC.SimpleFlyMovement(chargeLocationVelocity, chargeAcceleration);
                    }
                }

                NPC.netUpdate = true;

                if (NPC.netSpam > 10)
                    NPC.netSpam = 10;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        public override Color? GetAlpha(Color drawColor) => new Color(200, 150, 255) * NPC.Opacity;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            float chargePhaseGateValue = 480f;
            if (Main.getGoodWorld)
                chargePhaseGateValue *= 0.5f;

            float timeToReachFullColor = 120f;
            float colorChangeTime = 180f;
            float changeColorGateValue = chargePhaseGateValue - colorChangeTime;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color lightRed = new Color(255, 100, 100, 255) * NPC.Opacity;
            int afterimageAmt = 7;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;

                    if (!NPC.IsABestiaryIconDummy && Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > changeColorGateValue)
                        afterimageColor = Color.Lerp(afterimageColor, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - changeColorGateValue) / timeToReachFullColor, 0f, 1f));

                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Color color = NPC.GetAlpha(drawColor);

            if (!NPC.IsABestiaryIconDummy && Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > changeColorGateValue)
                color = Color.Lerp(color, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - changeColorGateValue) / timeToReachFullColor, 0f, 1f));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, color, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            Texture2D texture2D16 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterPhantomGlow").Value;
            Color c = Color.Red;
            if (!NPC.IsABestiaryIconDummy && (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] < changeColorGateValue || Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > changeColorGateValue))
                c = Color.Black;

            Color blackWhiteLerp = Color.Lerp(Color.White, c, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Vector2 otherAfterimagePos = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    otherAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    otherAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    Color otherAfterimageColor = blackWhiteLerp;
                    otherAfterimageColor = Color.Lerp(otherAfterimageColor, Color.White, 0.5f);
                    otherAfterimageColor = NPC.GetAlpha(otherAfterimageColor);
                    otherAfterimageColor *= (afterimageAmt - j) / 15f;
                    spriteBatch.Draw(texture2D16, otherAfterimagePos, NPC.frame, otherAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D16, drawLocation, NPC.frame, blackWhiteLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = NPC.frame.Y + frameHeight;
            }
            if (NPC.frame.Y > frameHeight * 3)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.MoonLeech, 360, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, 180, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 90;
                NPC.height = 90;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 10; i++)
                {
                    int ghostDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Polterplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[ghostDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[ghostDust].scale = 0.5f;
                        Main.dust[ghostDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 60; j++)
                {
                    int ghostDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[ghostDust2].noGravity = true;
                    Main.dust[ghostDust2].velocity *= 5f;
                    ghostDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[ghostDust2].velocity *= 2f;
                }
            }
        }
    }
}

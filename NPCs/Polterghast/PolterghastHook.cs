using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Polterghast
{
    public class PolterghastHook : ModNPC
    {
        private int despawnTimer = 300;
        private bool phase2 = false;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 40;
            NPC.height = 40;
            NPC.lifeMax = 50000;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit34;
            NPC.DeathSound = SoundID.NPCDeath39;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2);
            writer.Write(despawnTimer);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2 = reader.ReadBoolean();
            despawnTimer = reader.ReadInt32();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.3f, 0.3f);

            // Bools
            bool speedBoost = false;
            bool despawnBoost = false;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Despawn if Polter is gone
            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Player player = Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target];

            if (!player.active || player.dead)
            {
                speedBoost = true;
                despawnBoost = true;
            }

            float chargePhaseGateValue = 480f;
            if (Main.getGoodWorld)
                chargePhaseGateValue *= 0.5f;

            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= chargePhaseGateValue - 60f;

            // Percent life remaining, Polter
            float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / (float)Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];

            // Despawn
            if (CalamityGlobalNPC.ghostBoss != -1 && !player.ZoneDungeon &&
                player.position.Y < Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                NPC.localAI[0] -= 6f;
                speedBoost = true;
            }
            else
                despawnTimer++;

            // Phase 2
            bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
            phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f) && !phase3;
            if (phase2)
            {
                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.TargetClosest();

                // Despawn safety, make sure to target another player if the current player target is too far away
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                    NPC.TargetClosest();

                Movement(phase2, expertMode, revenge, death, speedBoost, despawnBoost, lifeRatio, tileEnrageMult, player);

                // Fire projectiles
                Vector2 hookPosition = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                float targetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - hookPosition.X;
                float targetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - hookPosition.Y;
                float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

                if (chargePhase)
                {
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    return;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[3] == 0f)
                {
                    if (NPC.ai[2] > ((CalamityWorld.LegendaryMode && revenge) ? 12f : 120f))
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 1f;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    if (NPC.ai[2] > 40f)
                        NPC.ai[3] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == 20f)
                    {
                        float shotSpeed = 10f * tileEnrageMult;
                        int type = ModContent.ProjectileType<PhantomHookShot>();
                        int damage = NPC.GetProjectileDamage(type);
                        targetDistance = shotSpeed / targetDistance;
                        targetX *= targetDistance;
                        targetY *= targetDistance;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), hookPosition.X, hookPosition.Y, targetX, targetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                return;
            }

            // Phase 1 or 3
            Movement(phase2, expertMode, revenge, death, speedBoost, despawnBoost, lifeRatio, tileEnrageMult, player);
        }

        private void Movement(bool phase2, bool expertMode, bool revenge, bool death, bool speedBoost, bool despawnBoost, float lifeRatio, float tileEnrageMult, Player player)
        {
            float chargePhaseGateValue = 480f;
            if (Main.getGoodWorld)
                chargePhaseGateValue *= 0.5f;

            float colorChangeTime = 180f;
            float changeColorGateValue = chargePhaseGateValue - colorChangeTime;

            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= chargePhaseGateValue - 60f;

            if (phase2)
            {
                float playerXDirection = Main.player[NPC.target].Center.X - NPC.Center.X;
                float playerYDirection = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                NPC.rotation = (float)Math.Atan2(playerYDirection, playerXDirection) + MathHelper.PiOver2;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                    NPC.ai[0] = (int)(NPC.Center.X / 16f);
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = (int)(NPC.Center.X / 16f);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                    NPC.localAI[0] = 0f;

                if (chargePhase)
                {
                    NPC.localAI[0] -= 10f;
                }
                else
                {
                    float shootBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                    NPC.localAI[0] -= 1f + shootBoost * tileEnrageMult;
                    if (expertMode)
                        NPC.localAI[0] -= Vector2.Distance(NPC.Center, player.Center) * 0.002f;
                    if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] >= changeColorGateValue)
                        NPC.localAI[0] -= 3f;
                    if (speedBoost)
                        NPC.localAI[0] -= 6f;
                }

                if (!despawnBoost && NPC.localAI[0] <= 0f && NPC.ai[0] != 0f)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].type == NPC.type && (Main.npc[i].velocity.X != 0f || Main.npc[i].velocity.Y != 0f))
                            NPC.localAI[0] = 180f;
                    }
                }

                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = 450f;
                    bool canMoveToTile = false;
                    int increment = 0;
                    while (!canMoveToTile && increment <= 1000)
                    {
                        increment++;
                        int playerTileX = (int)(player.Center.X / 16f);
                        int playerTileY = (int)(player.Center.Y / 16f);
                        if (NPC.ai[0] == 0f)
                        {
                            playerTileX = (int)((player.Center.X + Main.npc[CalamityGlobalNPC.ghostBoss].Center.X) / 32f);
                            playerTileY = (int)((player.Center.Y + Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y) / 32f);
                        }
                        if (despawnBoost)
                        {
                            playerTileX = (int)Main.npc[CalamityGlobalNPC.ghostBoss].position.X / 16;
                            playerTileY = (int)(Main.npc[CalamityGlobalNPC.ghostBoss].position.Y + 400f) / 16;
                        }
                        int randPlayerRadius = 20;
                        randPlayerRadius += (int)(100f * (increment / 1000f));
                        int randTileX = playerTileX + Main.rand.Next(-randPlayerRadius, randPlayerRadius + 1);
                        int randTileY = playerTileY + Main.rand.Next(-randPlayerRadius, randPlayerRadius + 1);
                        try
                        {
                            if (WorldGen.SolidTile(randTileX, randTileY) || Main.tile[randTileX, randTileY].WallType > 0 || chargePhase)
                            {
                                canMoveToTile = true;
                                NPC.ai[0] = randTileX;
                                NPC.ai[1] = randTileY;
                                NPC.localAI[1] = Vector2.Distance(NPC.Center, player.Center) * 0.01f;
                                NPC.netUpdate = true;
                            }
                        } catch
                        {
                        }
                    }
                }
            }

            if (NPC.ai[0] > 0f && NPC.ai[1] > 0f)
            {
                float velocityBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                float velocity = (8f + velocityBoost) * tileEnrageMult;
                if (expertMode)
                    velocity += NPC.localAI[1];
                if (revenge)
                    velocity += 1f;
                if (speedBoost)
                    velocity *= 2f;
                if (despawnBoost)
                    velocity *= 2f;

                Vector2 hookCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float hookXDest = NPC.ai[0] * 16f - 8f - hookCenter.X;
                float hookYDest = NPC.ai[1] * 16f - 8f - hookCenter.Y;
                float hookDestination = (float)Math.Sqrt(hookXDest * hookXDest + hookYDest * hookYDest);
                if (hookDestination < 12f + velocity)
                {
                    NPC.velocity.X = hookXDest;
                    NPC.velocity.Y = hookYDest;
                }
                else
                {
                    hookDestination = velocity / hookDestination;
                    NPC.velocity.X = hookXDest * hookDestination;
                    NPC.velocity.Y = hookYDest * hookDestination;
                }

                if (!phase2)
                {
                    Vector2 hookCenterPassive = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float polterDirectionX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - hookCenterPassive.X;
                    float polterDirectionY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - hookCenterPassive.Y;
                    NPC.rotation = (float)Math.Atan2(polterDirectionY, polterDirectionX) - 1.57f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CalamityGlobalNPC.ghostBoss < 0 || !NPC.active || NPC.IsABestiaryIconDummy)
                return true;

            Color lightRed = new Color(255, 100, 100, 255);

            float chargePhaseGateValue = 480f;
            if (Main.getGoodWorld)
                chargePhaseGateValue *= 0.5f;

            float timeToReachFullColor = 120f;
            float colorChangeTime = 180f;
            float changeColorGateValue = chargePhaseGateValue - colorChangeTime;

            if (Main.npc[CalamityGlobalNPC.ghostBoss].active && !phase2)
            {
                Vector2 center = NPC.Center;
                float bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
                float bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
                float chainRotation = (float)Math.Atan2(bossCenterY, bossCenterX) - 1.57f;
                bool draw = true;
                while (draw)
                {
                    int chainWidth = 20;
                    int chainHeight = 52;
                    float polterDistance = (float)Math.Sqrt(bossCenterX * bossCenterX + bossCenterY * bossCenterY);
                    if (polterDistance < chainHeight)
                    {
                        chainWidth = (int)polterDistance - chainHeight + chainWidth;
                        draw = false;
                    }
                    polterDistance = chainWidth / polterDistance;
                    bossCenterX *= polterDistance;
                    bossCenterY *= polterDistance;
                    center.X += bossCenterX;
                    center.Y += bossCenterY;
                    bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
                    bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;

                    Color cyanLerpColor = Color.Lerp(Color.White, Color.Cyan, 0.5f);
                    if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > changeColorGateValue)
                        cyanLerpColor = Color.Lerp(cyanLerpColor, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - changeColorGateValue) / timeToReachFullColor, 0f, 1f));

                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value.Width, chainWidth)), cyanLerpColor, chainRotation,
                        new Vector2(ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value.Width * 0.5f, ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            int afterimageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastHookGlow").Value;
            Color cyanLerp2 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > changeColorGateValue)
                cyanLerp2 = Color.Lerp(cyanLerp2, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - changeColorGateValue) / timeToReachFullColor, 0f, 1f));

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color otherAfterimageColor = cyanLerp2;
                    otherAfterimageColor = Color.Lerp(otherAfterimageColor, Color.White, 0.5f);
                    otherAfterimageColor = NPC.GetAlpha(otherAfterimageColor);
                    otherAfterimageColor *= (afterimageAmt - j) / 15f;
                    Vector2 otherAfterimagePos = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    otherAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    otherAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, otherAfterimagePos, NPC.frame, otherAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, cyanLerp2, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (phase2)
            {
                if (NPC.ai[3] == 0f)
                {
                    if (NPC.frame.Y < 1)
                    {
                        NPC.frameCounter += 1.0;
                        if (NPC.frameCounter > 4.0)
                        {
                            NPC.frameCounter = 0.0;
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                        }
                    }
                }
                else if (NPC.frame.Y > 0)
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 4.0)
                    {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y = NPC.frame.Y - frameHeight;
                    }
                }
                return;
            }
            if (NPC.velocity.X == 0f && NPC.velocity.Y == 0f)
            {
                if (NPC.frame.Y < 1)
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 4.0)
                    {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                    }
                }
            }
            else if (NPC.frame.Y > 0)
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 4.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y - frameHeight;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 180, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 180, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}

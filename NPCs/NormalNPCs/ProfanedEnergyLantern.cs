using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ProfanedEnergyLantern : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.ProfanedEnergyBody.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 1f;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 1;
            AIType = -1;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.energyFlame < 0 || !Main.npc[CalamityGlobalNPC.energyFlame].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }
            int energyBase = CalamityGlobalNPC.energyFlame;
            if (NPC.ai[3] > 0f)
            {
                energyBase = (int)NPC.ai[3] - 1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] -= 1f;
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = (float)Main.rand.Next(120, 480);
                    NPC.ai[0] = (float)Main.rand.Next(-100, 101);
                    NPC.ai[1] = (float)Main.rand.Next(-100, 101);
                    NPC.netUpdate = true;
                }
            }
            NPC.TargetClosest(true);
            float lanternAcceleration = 0.1f;
            float lanternSpeed = 500f;
            if ((double)Main.npc[CalamityGlobalNPC.energyFlame].life < (double)Main.npc[CalamityGlobalNPC.energyFlame].lifeMax * 0.25)
            {
                lanternSpeed += 50f;
            }
            if ((double)Main.npc[CalamityGlobalNPC.energyFlame].life < (double)Main.npc[CalamityGlobalNPC.energyFlame].lifeMax * 0.1)
            {
                lanternSpeed += 50f;
            }
            if (Main.expertMode)
            {
                float expertSpeedMult = 1f - (float)NPC.life / (float)NPC.lifeMax;
                lanternSpeed += expertSpeedMult * 200f;
                lanternAcceleration += 0.15f;
            }
            if (!Main.npc[energyBase].active || CalamityGlobalNPC.energyFlame < 0)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }
            Vector2 lanternRelocation = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            float targetXDist = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - (float)(NPC.width / 2) - lanternRelocation.X;
            float targetYDist = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - (float)(NPC.height / 2) - lanternRelocation.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
            float baseXPos = Main.npc[energyBase].position.X + (float)(Main.npc[energyBase].width / 2);
            float baseYPos = Main.npc[energyBase].position.Y + (float)(Main.npc[energyBase].height / 2);
            Vector2 baseCurrentPos = new Vector2(baseXPos, baseYPos);
            float basePosition = baseXPos + NPC.ai[0];
            float newLanternPos = baseYPos + NPC.ai[1];
            float baseXDist = basePosition - baseCurrentPos.X;
            float baseYDist = newLanternPos - baseCurrentPos.Y;
            float baseDistance = (float)Math.Sqrt((double)(baseXDist * baseXDist + baseYDist * baseYDist));
            baseDistance = lanternSpeed / baseDistance;
            baseXDist *= baseDistance;
            baseYDist *= baseDistance;
            if (NPC.position.X < baseXPos + baseXDist)
            {
                NPC.velocity.X = NPC.velocity.X + lanternAcceleration;
                if (NPC.velocity.X < 0f && baseXDist > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            else if (NPC.position.X > baseXPos + baseXDist)
            {
                NPC.velocity.X = NPC.velocity.X - lanternAcceleration;
                if (NPC.velocity.X > 0f && baseXDist < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            if (NPC.position.Y < baseYPos + baseYDist)
            {
                NPC.velocity.Y = NPC.velocity.Y + lanternAcceleration;
                if (NPC.velocity.Y < 0f && baseYDist > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.9f;
                }
            }
            else if (NPC.position.Y > baseYPos + baseYDist)
            {
                NPC.velocity.Y = NPC.velocity.Y - lanternAcceleration;
                if (NPC.velocity.Y > 0f && baseYDist < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.9f;
                }
            }
            if (NPC.velocity.X > 8f)
            {
                NPC.velocity.X = 8f;
            }
            if (NPC.velocity.X < -8f)
            {
                NPC.velocity.X = -8f;
            }
            if (NPC.velocity.Y > 8f)
            {
                NPC.velocity.Y = 8f;
            }
            if (NPC.velocity.Y < -8f)
            {
                NPC.velocity.Y = -8f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Main.player[NPC.target].dead)
                {
                    NPC.localAI[1] += (float)Main.rand.Next(1, 6);
                    if (NPC.localAI[1] >= 600f)
                    {
                        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            float projSpeed = 11f;
                            lanternRelocation = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                            targetXDist = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - lanternRelocation.X + (float)Main.rand.Next(-10, 11);
                            float absoluteTargetX = Math.Abs(targetXDist * 0.1f);
                            if (targetYDist > 0f)
                            {
                                absoluteTargetX = 0f;
                            }
                            targetYDist = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - lanternRelocation.Y + (float)Main.rand.Next(-10, 11) - absoluteTargetX;
                            targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                            targetDistance = projSpeed / targetDistance;
                            targetXDist *= targetDistance;
                            targetYDist *= targetDistance;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, targetXDist, targetYDist, ModContent.ProjectileType<HolyBomb>(), 40, 0f, Main.myPlayer, 0f, 0f);
                            NPC.localAI[1] = 0f;
                            return;
                        }
                        NPC.localAI[1] = 250f;
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CalamityGlobalNPC.energyFlame != -1)
            {
                Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
                float drawPositionX = Main.npc[CalamityGlobalNPC.energyFlame].Center.X - center.X;
                float drawPositionY = Main.npc[CalamityGlobalNPC.energyFlame].Center.Y - center.Y;
                drawPositionY += 10f;
                float rotation = (float)Math.Atan2((double)drawPositionY, (double)drawPositionX) - 1.57f;
                bool draw = true;
                while (draw)
                {
                    float totalDrawDistance = (float)Math.Sqrt((double)(drawPositionX * drawPositionX + drawPositionY * drawPositionY));
                    if (totalDrawDistance < 16f)
                    {
                        draw = false;
                    }
                    else
                    {
                        totalDrawDistance = 16f / totalDrawDistance;
                        drawPositionX *= totalDrawDistance;
                        drawPositionY *= totalDrawDistance;
                        center.X += drawPositionX;
                        center.Y += drawPositionY;
                        drawPositionX = Main.npc[CalamityGlobalNPC.energyFlame].Center.X - center.X;
                        drawPositionY = Main.npc[CalamityGlobalNPC.energyFlame].Center.Y - center.Y;
                        drawPositionY -= 10f;
                        Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                        Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/NPCs/NormalNPCs/ProfanedEnergySegment").Value;
                        Main.spriteBatch.Draw(chain, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                            new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, chain.Width, chain.Height)), color, rotation,
                            new Vector2((float)chain.Width * 0.5f, (float)chain.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}

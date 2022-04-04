using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ProfanedEnergyLantern : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Energy");
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
                NPC.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
                NPC.netUpdate = true;
                return;
            }
            int num750 = CalamityGlobalNPC.energyFlame;
            if (NPC.ai[3] > 0f)
            {
                num750 = (int)NPC.ai[3] - 1;
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
            float num751 = 0.1f;
            float num752 = 500f;
            if ((double)Main.npc[CalamityGlobalNPC.energyFlame].life < (double)Main.npc[CalamityGlobalNPC.energyFlame].lifeMax * 0.25)
            {
                num752 += 50f;
            }
            if ((double)Main.npc[CalamityGlobalNPC.energyFlame].life < (double)Main.npc[CalamityGlobalNPC.energyFlame].lifeMax * 0.1)
            {
                num752 += 50f;
            }
            if (Main.expertMode)
            {
                float num753 = 1f - (float)NPC.life / (float)NPC.lifeMax;
                num752 += num753 * 200f;
                num751 += 0.15f;
            }
            if (!Main.npc[num750].active || CalamityGlobalNPC.energyFlame < 0)
            {
                NPC.active = false;
                return;
            }
            Vector2 vector22 = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            float num189 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - (float)(NPC.width / 2) - vector22.X;
            float num190 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - (float)(NPC.height / 2) - vector22.Y;
            float num191 = (float)Math.Sqrt((double)(num189 * num189 + num190 * num190));
            float num754 = Main.npc[num750].position.X + (float)(Main.npc[num750].width / 2);
            float num755 = Main.npc[num750].position.Y + (float)(Main.npc[num750].height / 2);
            Vector2 vector93 = new Vector2(num754, num755);
            float num756 = num754 + NPC.ai[0];
            float num757 = num755 + NPC.ai[1];
            float num758 = num756 - vector93.X;
            float num759 = num757 - vector93.Y;
            float num760 = (float)Math.Sqrt((double)(num758 * num758 + num759 * num759));
            num760 = num752 / num760;
            num758 *= num760;
            num759 *= num760;
            if (NPC.position.X < num754 + num758)
            {
                NPC.velocity.X = NPC.velocity.X + num751;
                if (NPC.velocity.X < 0f && num758 > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            else if (NPC.position.X > num754 + num758)
            {
                NPC.velocity.X = NPC.velocity.X - num751;
                if (NPC.velocity.X > 0f && num758 < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            if (NPC.position.Y < num755 + num759)
            {
                NPC.velocity.Y = NPC.velocity.Y + num751;
                if (NPC.velocity.Y < 0f && num759 > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y * 0.9f;
                }
            }
            else if (NPC.position.Y > num755 + num759)
            {
                NPC.velocity.Y = NPC.velocity.Y - num751;
                if (NPC.velocity.Y > 0f && num759 < 0f)
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
                            float num196 = 11f;
                            vector22 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                            num189 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector22.X + (float)Main.rand.Next(-10, 11);
                            float num197 = Math.Abs(num189 * 0.1f);
                            if (num190 > 0f)
                            {
                                num197 = 0f;
                            }
                            num190 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector22.Y + (float)Main.rand.Next(-10, 11) - num197;
                            num191 = (float)Math.Sqrt((double)(num189 * num189 + num190 * num190));
                            num191 = num196 / num191;
                            num189 *= num191;
                            num190 *= num191;
                            int num9 = ModContent.ProjectileType<HolyBomb>();
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center.X, NPC.Center.Y, num189, num190, num9, 40, 0f, Main.myPlayer, 0f, 0f);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
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
                        Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/NPCs/NormalNPCs/ProfanedEnergySegment");
                        Main.spriteBatch.Draw(chain, new Vector2(center.X - Main.screenPosition.X, center.Y - Main.screenPosition.Y),
                            new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, chain.Width, chain.Height)), color, rotation,
                            new Vector2((float)chain.Width * 0.5f, (float)chain.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}

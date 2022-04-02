using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TenebreusTidesWaterSpear : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/UndinesRetributionSpear";

        private int penetrationAmt = 4;
        private bool dontDraw = false;
        private int drawInt = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tenebreus Tides Water Spear");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = penetrationAmt;
            projectile.timeLeft = 600;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(penetrationAmt);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            penetrationAmt = reader.ReadInt32();
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);

            // If projectile hasn't hit anything yet
            if (projectile.ai[0] == 0f)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 7f)
                {
                    int water = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.4f);
                    Main.dust[water].noGravity = true;
                    Main.dust[water].velocity *= 0.5f;
                    Main.dust[water].velocity += projectile.velocity * 0.1f;
                }
                float scalar = 0.01f;
                int alphaAmt = 5;
                int alphaCeiling = alphaAmt * 15;
                int alphaFloor = 0;
                if (projectile.localAI[0] > 7f)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.scale -= scalar;

                        projectile.alpha += alphaAmt;
                        if (projectile.alpha > alphaCeiling)
                        {
                            projectile.alpha = alphaCeiling;
                            projectile.localAI[1] = 1f;
                        }
                    }
                    else if (projectile.localAI[1] == 1f)
                    {
                        projectile.scale += scalar;

                        projectile.alpha -= alphaAmt;
                        if (projectile.alpha <= alphaFloor)
                        {
                            projectile.alpha = alphaFloor;
                            projectile.localAI[1] = 0f;
                        }
                    }
                }
            }

            // If projectile has hit an enemy and has 'split'
            else if (projectile.ai[0] >= 1f && projectile.ai[0] < (float)(1 + penetrationAmt))
            {
                projectile.alpha += 15;
                projectile.velocity *= 0.98f;
                projectile.localAI[0] = 0f;

                if (projectile.alpha >= 255)
                {
                    if (projectile.ai[0] == 1f)
                    {
                        projectile.Kill();
                        return;
                    }

                    int whoAmI = -1;
                    Vector2 targetSpot = projectile.Center;
                    float detectRange = 700f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                            if (targetDist < detectRange)
                            {
                                detectRange = targetDist;
                                targetSpot = npc.Center;
                                whoAmI = i;
                            }
                        }
                    }

                    if (whoAmI >= 0)
                    {
                        projectile.netUpdate = true;
                        projectile.ai[0] += (float)penetrationAmt;
                        projectile.position = targetSpot + ((float)Main.rand.NextDouble() * 6.28318548f).ToRotationVector2() * 100f - new Vector2((float)projectile.width, (float)projectile.height) / 2f;
                        dontDraw = true;
                        projectile.velocity = Vector2.Normalize(targetSpot - projectile.Center) * 18f;
                    }
                    else
                        projectile.Kill();
                }

                if (Main.rand.NextBool(3))
                {
                    int water = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.4f);
                    Main.dust[water].noGravity = true;
                    Main.dust[water].velocity *= 0.5f;
                    Main.dust[water].velocity += projectile.velocity * 0.1f;
                }
            }

            // If 'split' projectile has a target
            else if (projectile.ai[0] >= (float)(1 + penetrationAmt) && projectile.ai[0] < (float)(1 + penetrationAmt * 2))
            {
                projectile.scale = 0.9f;

                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 15f)
                {
                    projectile.alpha += 51;
                    projectile.velocity *= 0.8f;

                    if (projectile.alpha >= 255)
                        projectile.Kill();
                }
                else
                {
                    projectile.alpha -= 125;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.velocity *= 0.98f;
                }

                projectile.localAI[0] += 1f;

                int water = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.4f);
                Main.dust[water].noGravity = true;
                Main.dust[water].velocity *= 0.5f;
                Main.dust[water].velocity += projectile.velocity * 0.1f;
            }

            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0f, (255 - projectile.alpha) * 1f / 255f);
            if (dontDraw)
                drawInt++;
            if (drawInt > 1)
            {
                drawInt = 0;
                dontDraw = false;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(50, 50, 255, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (dontDraw)
                return false;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage()
        {
            // Do not do damage if a tile is hit OR if projectile has 'split' and hasn't been live for more than 5 frames
            if ((((int)(projectile.ai[0] - 1f) / penetrationAmt == 0 && penetrationAmt < 3) || projectile.ai[1] < 5f) && projectile.ai[0] != 0f)
                return false;
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // If 'split' projectile hits an enemy
            if (projectile.ai[0] >= (float)(1 + penetrationAmt) && projectile.ai[0] < (float)(1 + penetrationAmt * 2))
                projectile.ai[0] = 0f;

            // Becomes 5 on first hit, then 4, and so on
            penetrationAmt--;

            // Hits enemy, ai[0] = 0f + 4f = 4f on first hit
            // ai[0] = 4f - 1f = 3f on second hit
            // ai[0] = 3f - 1f = 2f on third hit
            if (projectile.ai[0] == 0f)
                projectile.ai[0] += (float)penetrationAmt;
            else
                projectile.ai[0] -= (float)(penetrationAmt + 1);

            projectile.ai[1] = 0f;
            projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }
    }
}

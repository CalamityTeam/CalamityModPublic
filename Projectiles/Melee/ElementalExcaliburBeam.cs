using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles
{
    public class ElementalExcaliburBeam : ModProjectile
    {
        private int alpha = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item60, projectile.position);
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.02f;
                projectile.alpha += 30;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.02f;
                projectile.alpha -= 30;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }

            Color color = new Color(255, 0, 0, alpha);
            switch ((int)projectile.ai[0])
            {
                case 0: // Red, normal beam
                    break;

                case 1: // Orange, curve back to player
                    color = new Color(255, 128, 0, alpha);

                    int num123 = (int)Player.FindClosest(projectile.Center, 1, 1);
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] < 110f && projectile.ai[1] > 30f)
                    {
                        float scaleFactor2 = projectile.velocity.Length();
                        Vector2 vector17 = Main.player[num123].Center - projectile.Center;
                        vector17.Normalize();
                        vector17 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 24f + vector17) / 25f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;
                    }

                    if (projectile.velocity.Length() < 24f)
                    {
                        projectile.velocity *= 1.02f;
                    }

                    break;

                case 2: // Yellow, split after a certain time
                    color = new Color(255, 255, 0, alpha);

                    projectile.localAI[1] += 1f;
                    if (projectile.localAI[1] >= 90f)
                    {
                        projectile.localAI[1] = 0f;
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(10);
                        if (projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj + 1; i++)
                            {
                                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X * 0.5f, perturbedSpeed.Y * 0.5f, ModContent.ProjectileType<ElementalExcaliburBeam>(), (int)((double)projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                            }
                        }
                        projectile.Kill();
                    }

                    break;

                case 3: // Lime, home in on player within certain distance
                    color = new Color(128, 255, 0, alpha);

                    float num953 = 75f;
                    float scaleFactor12 = 15f;
                    float num954 = 80f;
                    if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
                    {
                        if (projectile.Distance(Main.player[projectile.owner].Center) > num954)
                        {
                            Vector2 vector102 = projectile.DirectionTo(Main.player[projectile.owner].Center);
                            if (vector102.HasNaNs())
                            {
                                vector102 = Vector2.UnitY;
                            }
                            projectile.velocity = (projectile.velocity * (num953 - 1f) + vector102 * scaleFactor12) / num953;
                        }
                    }
                    else
                    {
                        if (projectile.timeLeft > 30)
                        {
                            projectile.timeLeft = 30;
                        }
                    }

                    break;

                case 4: // Green, home in on enemies
                    color = new Color(0, 255, 0, alpha);

                    float num472 = projectile.Center.X;
                    float num473 = projectile.Center.Y;
                    float num474 = 600f;
                    bool flag17 = false;
                    for (int num475 = 0; num475 < 200; num475++)
                    {
                        if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
                        {
                            float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                            float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                            float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                            if (num478 < num474)
                            {
                                num474 = num478;
                                num472 = num476;
                                num473 = num477;
                                flag17 = true;
                            }
                        }
                    }
                    if (flag17)
                    {
                        float num483 = 12f;
                        Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num484 = num472 - vector35.X;
                        float num485 = num473 - vector35.Y;
                        float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                        num486 = num483 / num486;
                        num484 *= num486;
                        num485 *= num486;
                        projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                        projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
                    }

                    break;

                case 5: // Turquoise, speed up and don't collide with tiles
                    color = new Color(0, 255, 128, alpha);

                    projectile.tileCollide = false;
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 28f)
                    {
                        projectile.velocity *= 1.035f;
                    }

                    break;

                case 6: // Cyan, upon death create several turquoise swords
                    color = new Color(0, 255, 255, alpha);
                    break;

                case 7: // Light Blue, slow down on hit
                    color = new Color(0, 128, 255, alpha);
                    break;

                case 8: // Blue, bounce off of tiles and enemies
                    color = new Color(0, 0, 255, alpha);
                    break;

                case 9: // Purple, speed up and slow down over and over
                    color = new Color(128, 0, 255, alpha);

                    projectile.localAI[1] += 1f;
                    if (projectile.localAI[1] <= 20f)
                    {
                        projectile.velocity *= 0.95f;
                    }
                    else if (projectile.localAI[1] > 20f && projectile.localAI[1] <= 39f)
                    {
                        projectile.velocity *= 1.05f;
                    }
                    else if (projectile.localAI[1] == 40f)
                    {
                        projectile.localAI[1] = 0f;
                    }

                    break;

                case 10: // Fuschia, start slow then get faster
                    color = new Color(255, 0, 255, alpha);

                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.velocity *= 0.1f;
                        projectile.localAI[1] += 1f;
                    }
                    projectile.velocity *= 1.01f;

                    break;

                case 11: // Hot Pink, split into fuschia while travelling
                    color = new Color(255, 0, 128, alpha);

                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.velocity *= 0.33f;
                    }
                    projectile.localAI[1] += 1f;
                    if (projectile.localAI[1] >= 91f)
                    {
                        projectile.localAI[1] = 1f;
                        if (Main.myPlayer == projectile.owner)
                            Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<ElementalExcaliburBeam>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 10f, 0f);
                    }

                    break;

                default:
                    break;
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 240);
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.CursedInferno, 240);
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(BuffID.OnFire, 240);
            target.AddBuff(BuffID.Ichor, 240);

            if (projectile.ai[0] == 7f)
            {
                projectile.velocity *= 0.25f;
            }
            else if (projectile.ai[0] == 8f)
            {
                projectile.velocity *= -1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[0] == 8f)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = new Color(255, 0, 0, alpha);
            switch ((int)projectile.ai[0])
            {
                case 0: // Red
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, alpha);
                    break;
                case 6: // Cyan
                    color = new Color(0, 255, 255, alpha);
                    break;
                case 7: // Light Blue
                    color = new Color(0, 128, 255, alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, alpha);
                    break;
                default:
                    break;
            }
            return color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 590)
                return false;

            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            Color color = new Color(255, 0, 0, alpha);
            switch ((int)projectile.ai[0])
            {
                case 0: // Red
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, alpha);
                    break;

                case 6: // Cyan
                    color = new Color(0, 255, 255, alpha);

                    for (int x = 0; x < 3; x++)
                    {
                        float xPos = projectile.ai[0] > 0 ? projectile.position.X + 500 : projectile.position.X - 500;
                        Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-500, 501));
                        float num80 = xPos;
                        float speedX = projectile.Center.X - vector2.X;
                        float speedY = projectile.Center.Y - vector2.Y;
                        float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                        dir = 5 / num80;
                        speedX *= dir * 150;
                        speedY *= dir * 150;
                        if (projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, ModContent.ProjectileType<ElementalExcaliburBeam>(), (int)((double)projectile.damage * 0.2), 2f, projectile.owner, 5f, 0f);
                        }
                    }

                    break;

                case 7: // Light Blue
                    color = new Color(0, 128, 255, alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, alpha);
                    break;
                default:
                    break;
            }

            for (int num193 = 0; num193 < 3; num193++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int num194 = 0; num194 < 30; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
}

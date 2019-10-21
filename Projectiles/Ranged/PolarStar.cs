using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class PolarStar : ModProjectile
    {
        private int dust1 = 86;
        private int dust2 = 91;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polar Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
        }

        public override void AI()
        {
            //Looking pretty at boost two or three
            if (projectile.ai[1] == 2f || projectile.ai[1] == 1f)
            {
                Vector2 value7 = new Vector2(5f, 10f);
                Lighting.AddLight(projectile.Center, 0.25f, 0f, 0.25f);
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] == 48f)
                {
                    projectile.localAI[0] = 0f;
                }
                else
                {
                    for (int num41 = 0; num41 < 2; num41++)
                    {
                        int dustType = num41 == 0 ? dust1 : dust2;
                        Vector2 value8 = Vector2.UnitX * -12f;
                        value8 = -Vector2.UnitY.RotatedBy((double)(projectile.localAI[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7;
                        int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = dustType == dust1 ? 1.5f : 1f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = projectile.Center + value8;
                        Main.dust[num42].velocity = projectile.velocity;
                        int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                        Main.dust[num458].noGravity = true;
                        Main.dust[num458].velocity *= 0f;
                    }
                }
            }
            if (projectile.ai[1] == 2f) //Boost three
            {
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
                    float num483 = 14f;
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
            }
            else if (projectile.ai[1] == 1f) //Boost two
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 25;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                float num55 = 30f;
                float num56 = 1f;
                if (projectile.ai[1] == 1f)
                {
                    projectile.localAI[0] += num56;
                    if (projectile.localAI[0] > num55)
                    {
                        projectile.localAI[0] = num55;
                    }
                }
                else
                {
                    projectile.localAI[0] -= num56;
                    if (projectile.localAI[0] <= 0f)
                    {
                        projectile.Kill();
                    }
                }
            }
            else //No boosts
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 25;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                float num55 = 40f;
                float num56 = 1.5f;
                if (projectile.ai[1] == 0f)
                {
                    projectile.localAI[0] += num56;
                    if (projectile.localAI[0] > num55)
                    {
                        projectile.localAI[0] = num55;
                    }
                }
                else
                {
                    projectile.localAI[0] -= num56;
                    if (projectile.localAI[0] <= 0f)
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            int num147 = 0;
            int num148 = 0;
            float num149 = (float)(Main.projectileTexture[projectile.type].Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Rectangle value6 = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
            if (projectile.getRect().Intersects(value6))
            {
                Vector2 value7 = new Vector2(projectile.position.X - Main.screenPosition.X + num149 + (float)num148, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY);
                float num162 = 40f;
                float scaleFactor = 1.5f;
                if (projectile.ai[1] == 1f)
                {
                    num162 = (float)(int)projectile.localAI[0];
                }
                for (int num163 = 1; num163 <= (int)projectile.localAI[0]; num163++)
                {
                    Vector2 value8 = Vector2.Normalize(projectile.velocity) * (float)num163 * scaleFactor;
                    Color color29 = projectile.GetAlpha(color25);
                    color29 *= (num162 - (float)num163) / num162;
                    color29.A = 0;
                    Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], value7 - value8, null, color29, projectile.rotation, new Vector2(num149, (float)(projectile.height / 2 + num147)), projectile.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                player.AddBuff(ModContent.BuffType<PolarisBuff>(), 180);
            }
            else
            {
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 62, 0.5f);
            if (projectile.ai[1] == 1f) //Boost two
            {
                int projectiles = Main.rand.Next(2, 5);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < projectiles; k++)
                    {
                        int split = Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-10, 11) * 2f, (float)Main.rand.Next(-10, 11) * 2f, ModContent.ProjectileType<ChargedBlast2>(),
                        (int)((double)projectile.damage * 0.85), (float)(int)((double)projectile.knockBack * 0.5), Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (projectile.ai[1] == 2f) //Boost three
            {
                int height = 150;

                projectile.position = projectile.Center;
                projectile.width = projectile.height = height;
                projectile.Center = projectile.position;
                projectile.maxPenetrate = -1;
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.damage /= 2;
                projectile.Damage();

                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++) // 108 dusts
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.3f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;

                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, vector7.X * 0.3f, vector7.Y * 0.3f, 100, default, 2f);
                    Main.dust[num228].noGravity = true;

                    int num229 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, vector7.X * 0.2f, vector7.Y * 0.2f, 100, default, 2f);
                    Main.dust[num229].noGravity = true;

                    int num230 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, vector7.X * 0.1f, vector7.Y * 0.1f, 100, default, 2f);
                    Main.dust[num230].noGravity = true;
                }

                bool random = Main.rand.NextBool();
                float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f) // 125 dusts
                {
                    random = !random;
                    Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                    Dust d = Dust.NewDustPerfect(projectile.Center, random ? dust1 : dust2, velocity);
                    d.noGravity = true;
                    d.customData = 0.025f;
                    d.scale = 2f;
                }
            }
        }
    }
}

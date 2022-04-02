using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DestructionBolt : ModProjectile
    {
        public int dustType = 191;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Destruction Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Main.rand.Next(8) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            if (projectile.alpha > 0)
                projectile.alpha -= 8;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            float num29 = 5f;
            float num30 = 300f;
            float scaleFactor = 6f;
            Vector2 value7 = new Vector2(10f, 20f);
            int num32 = 3 * projectile.MaxUpdates;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (projectile.ai[1] == 1f && projectile.owner == Main.myPlayer)
            {
                if (projectile.alpha < 128)
                {
                    int num35 = -1;
                    float num36 = num30;
                    for (int num37 = 0; num37 < Main.maxNPCs; num37++)
                    {
                        if (Main.npc[num37].active && Main.npc[num37].CanBeChasedBy(projectile, false))
                        {
                            Vector2 center3 = Main.npc[num37].Center;
                            float num38 = Vector2.Distance(center3, projectile.Center);
                            if (num38 < num36 && num35 == -1 && Collision.CanHitLine(projectile.Center, 1, 1, center3, 1, 1))
                            {
                                num36 = num38;
                                num35 = num37;
                            }
                        }
                    }
                    if (num36 < 4f)
                    {
                        projectile.Kill();
                        return;
                    }
                    if (num35 != -1)
                    {
                        projectile.ai[1] = num29 + 1f;
                        projectile.ai[0] = (float)num35;
                        projectile.netUpdate = true;
                    }
                }
            }
            else if (projectile.ai[1] > num29)
            {
                projectile.ai[1] += 1f;
                int num39 = (int)projectile.ai[0];
                if (!Main.npc[num39].active || !Main.npc[num39].CanBeChasedBy(projectile, false))
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.velocity.ToRotation();
                    Vector2 vector6 = Main.npc[num39].Center - projectile.Center;
                    if (vector6.Length() < 10f)
                    {
                        projectile.Kill();
                        return;
                    }
                    if (vector6 != Vector2.Zero)
                    {
                        vector6.Normalize();
                        vector6 *= scaleFactor;
                    }
                    float num40 = 30f;
                    projectile.velocity = (projectile.velocity * (num40 - 1f) + vector6) / num40;
                }
            }
            if (projectile.ai[1] >= 1f && projectile.ai[1] < num29)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] == num29)
                {
                    projectile.ai[1] = 1f;
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 48f)
            {
                projectile.localAI[0] = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override bool CanDamage()
        {
            return projectile.alpha < 128;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 50;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 45; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 9);
        }
    }
}

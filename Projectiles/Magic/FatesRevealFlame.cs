using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FatesRevealFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fate's Reveal");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 180;
            projectile.magic = true;
        }

        public override void AI()
        {
            float num949 = 10f;
            float scaleFactor11 = 15f;
            float num950 = 160f;
            if (projectile.timeLeft > 30 && projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.timeLeft > 30 && projectile.alpha < 128 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.alpha = 128;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            int num3 = projectile.frameCounter + 1;
            projectile.frameCounter = num3;
            if (num3 > 4)
            {
                projectile.frameCounter = 0;
                num3 = projectile.frame + 1;
                projectile.frame = num3;
                if (num3 >= 4)
                {
                    projectile.frame = 0;
                }
            }
            float num951 = 0.5f;
            if (projectile.timeLeft < 120)
            {
                num951 = 1.1f;
            }
            if (projectile.timeLeft < 60)
            {
                num951 = 1.6f;
            }
            float[] var_2_2A211_cp_0 = projectile.ai;
            int var_2_2A211_cp_1 = 1;
            float num73 = var_2_2A211_cp_0[var_2_2A211_cp_1];
            var_2_2A211_cp_0[var_2_2A211_cp_1] = num73 + 1f;
            float num952 = projectile.ai[1] / 180f * 6.28318548f;
            for (float num953 = 0f; num953 < 3f; num953 = num73 + 1f)
            {
                if (Main.rand.Next(3) != 0)
                {
                    return;
                }
                Dust dust13 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 60, 0f, -2f, 0, default, 1f)];
                dust13.position = projectile.Center + Vector2.UnitY.RotatedBy((double)(num953 * 6.28318548f / 3f + projectile.ai[1]), default) * 10f;
                dust13.noGravity = true;
                dust13.velocity = projectile.DirectionFrom(dust13.position);
                dust13.scale = num951;
                dust13.fadeIn = 0.5f;
                dust13.alpha = 200;
                num73 = num953;
            }
            if (projectile.timeLeft < 4)
            {
                projectile.position = projectile.Center;
                projectile.width = projectile.height = 180;
                projectile.Center = projectile.position;
                // Ozzatron 15FEB2021 -- What the actual fuck is this
                // projectile.damage = 800;
                for (int num955 = 0; num955 < 10; num955 = num3 + 1)
                {
                    Dust dust13 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, 0f, -2f, 0, default, 1f)];
                    dust13.noGravity = true;
                    if (dust13.position != projectile.Center)
                    {
                        dust13.velocity = projectile.DirectionTo(dust13.position) * 3f;
                    }
                    num3 = num955;
                }
            }
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
            {
                if (projectile.Distance(Main.player[projectile.owner].Center) > num950)
                {
                    Vector2 vector118 = projectile.DirectionTo(Main.player[projectile.owner].Center);
                    if (vector118.HasNaNs())
                    {
                        vector118 = Vector2.UnitY;
                    }
                    projectile.velocity = (projectile.velocity * (num949 - 1f) + vector118 * scaleFactor11) / num949;
                }
            }
            else
            {
                if (projectile.timeLeft > 30)
                {
                    projectile.timeLeft = 30;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 30)
            {
                float num4 = (float)projectile.timeLeft / 30f;
                projectile.alpha = (int)(255f - 255f * num4);
            }
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 128 - projectile.alpha / 2);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);

            projectile.position = projectile.Center;
            projectile.width = projectile.height = 84;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Damage();

            // Dust code
            int num3;
            for (int num122 = 0; num122 < 3; num122 = num3 + 1)
            {
                int num123 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num123].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                num3 = num122;
            }
            for (int num124 = 0; num124 < 10; num124 = num3 + 1)
            {
                int num125 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[num125].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                Main.dust[num125].noGravity = true;
                Dust dust = Main.dust[num125];
                dust.velocity *= 2f;
                num3 = num124;
            }
            for (int num126 = 0; num126 < 5; num126 = num3 + 1)
            {
                int num127 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Main.dust[num127].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                Main.dust[num127].noGravity = true;
                Dust dust = Main.dust[num127];
                dust.velocity *= 2f;
                num3 = num126;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class FatesRevealFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fate's Reveal");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            float num949 = 10f;
            float scaleFactor11 = 15f;
            float num950 = 160f;
            if (Projectile.timeLeft > 30 && Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.timeLeft > 30 && Projectile.alpha < 128 && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.alpha = 128;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            int num3 = Projectile.frameCounter + 1;
            Projectile.frameCounter = num3;
            if (num3 > 4)
            {
                Projectile.frameCounter = 0;
                num3 = Projectile.frame + 1;
                Projectile.frame = num3;
                if (num3 >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            float num951 = 0.5f;
            if (Projectile.timeLeft < 120)
            {
                num951 = 1.1f;
            }
            if (Projectile.timeLeft < 60)
            {
                num951 = 1.6f;
            }
            float[] var_2_2A211_cp_0 = Projectile.ai;
            int var_2_2A211_cp_1 = 1;
            float num73 = var_2_2A211_cp_0[var_2_2A211_cp_1];
            var_2_2A211_cp_0[var_2_2A211_cp_1] = num73 + 1f;
            float num952 = Projectile.ai[1] / 180f * 6.28318548f;
            for (float num953 = 0f; num953 < 3f; num953 = num73 + 1f)
            {
                if (Main.rand.Next(3) != 0)
                {
                    return;
                }
                Dust dust13 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 60, 0f, -2f, 0, default, 1f)];
                dust13.position = Projectile.Center + Vector2.UnitY.RotatedBy((double)(num953 * 6.28318548f / 3f + Projectile.ai[1]), default) * 10f;
                dust13.noGravity = true;
                dust13.velocity = Projectile.DirectionFrom(dust13.position);
                dust13.scale = num951;
                dust13.fadeIn = 0.5f;
                dust13.alpha = 200;
                num73 = num953;
            }
            if (Projectile.timeLeft < 4)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 180;
                Projectile.Center = Projectile.position;
                // Ozzatron 15FEB2021 -- What the actual fuck is this
                // projectile.damage = 800;
                for (int num955 = 0; num955 < 10; num955 = num3 + 1)
                {
                    Dust dust13 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, 0f, -2f, 0, default, 1f)];
                    dust13.noGravity = true;
                    if (dust13.position != Projectile.Center)
                        dust13.velocity = Projectile.SafeDirectionTo(dust13.position) * 3f;

                    num3 = num955;
                }
            }
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > num950)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (num949 - 1f) + moveDirection * scaleFactor11) / num949;
                }
            }
            else
            {
                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 30)
            {
                float num4 = (float)Projectile.timeLeft / 30f;
                Projectile.alpha = (int)(255f - 255f * num4);
            }
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 128 - Projectile.alpha / 2);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 84;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Damage();

            // Dust code
            int num3;
            for (int num122 = 0; num122 < 3; num122 = num3 + 1)
            {
                int num123 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num123].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                num3 = num122;
            }
            for (int num124 = 0; num124 < 10; num124 = num3 + 1)
            {
                int num125 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[num125].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[num125].noGravity = true;
                Dust dust = Main.dust[num125];
                dust.velocity *= 2f;
                num3 = num124;
            }
            for (int num126 = 0; num126 < 5; num126 = num3 + 1)
            {
                int num127 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Main.dust[num127].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                Main.dust[num127].noGravity = true;
                Dust dust = Main.dust[num127];
                dust.velocity *= 2f;
                num3 = num126;
            }
        }
    }
}

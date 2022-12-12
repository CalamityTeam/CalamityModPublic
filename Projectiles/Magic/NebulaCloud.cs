using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulaCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Cloud");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 116;
            Projectile.height = 116;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 720;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            int num925 = ModContent.ProjectileType<NebulaCloudCore>();
            float num926 = 720f;
            float x5 = 0.15f;
            float y4 = 0.15f;
            bool flag50 = false;
            bool flag51 = false;

            if (flag51)
            {
                int num927 = (int)Projectile.ai[1];
                if (!Main.projectile[num927].active || Main.projectile[num927].type != num925)
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.timeLeft = 2;
            }

            Projectile.ai[0]++;
            if (!(Projectile.ai[0] < num926))
                return;

            bool flag52 = true;
            int num928 = (int)Projectile.ai[1];
            if (Main.projectile[num928].active && Main.projectile[num928].type == num925)
            {
                if (!flag50 && Main.projectile[num928].oldPos[1] != Vector2.Zero)
                    Projectile.position += Main.projectile[num928].position - Main.projectile[num928].oldPos[1];

                if (Projectile.Center.HasNaNs())
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile.ai[0] = num926;
                flag52 = false;
                Projectile.Kill();
            }

            if (flag52 && !flag50)
            {
                Projectile.velocity += new Vector2(Math.Sign(Main.projectile[num928].Center.X - Projectile.Center.X), Math.Sign(Main.projectile[num928].Center.Y - Projectile.Center.Y)) * new Vector2(x5, y4);
                if (Projectile.velocity.Length() > 6f)
                    Projectile.velocity *= 6f / Projectile.velocity.Length();
            }

            if (Main.rand.NextBool())
            {
                int num929 = Dust.NewDust(Projectile.Center, 8, 8, 86);
                Main.dust[num929].position = Projectile.Center;
                Main.dust[num929].velocity = Projectile.velocity;
                Main.dust[num929].noGravity = true;
                Main.dust[num929].scale = 1.75f;

                if (flag52)
                    Main.dust[num929].customData = Main.projectile[(int)Projectile.ai[1]];
            }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.ai[0] = 86f;

            for (int num164 = 0; num164 < 15; num164++)
            {
                int num165 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default(Color), 0.75f);
                Dust dust;
                if (Main.rand.NextBool(3))
                {
                    Main.dust[num165].fadeIn = 0.75f + Main.rand.Next(-10, 11) * 0.015f;
                    Main.dust[num165].scale = 0.325f + Main.rand.Next(-10, 11) * 0.0075f;
                    dust = Main.dust[num165];
                    dust.type++;
                }
                else
                    Main.dust[num165].scale = 1.5f + Main.rand.Next(-10, 11) * 0.015f;

                Main.dust[num165].noGravity = true;
                dust = Main.dust[num165];
                dust.velocity *= 1.375f;
                dust = Main.dust[num165];
                dust.velocity -= Projectile.oldVelocity / 20f;
            }
        }
    }
}

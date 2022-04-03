using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GoldenGunProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Round");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 5f)
            {
                Projectile.localAI[0] += 1f;
                return;
            }
            int num3;
            for (int num586 = 0; num586 < 1; num586 = num3 + 1)
            {
                for (int num587 = 0; num587 < 6; num587 = num3 + 1)
                {
                    float num588 = Projectile.velocity.X / 6f * (float)num587;
                    float num589 = Projectile.velocity.Y / 6f * (float)num587;
                    int num590 = 6;
                    int num591 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num590, Projectile.position.Y + (float)num590), Projectile.width - num590 * 2, Projectile.height - num590 * 2, 170, 0f, 0f, 75, default, 1.2f);
                    Dust dust = Main.dust[num591];
                    if (Main.rand.NextBool(2))
                    {
                        dust.alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        dust.alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        dust.alpha += 25;
                    }
                    dust.noGravity = true;
                    dust.velocity *= 0.3f;
                    dust.velocity += Projectile.velocity * 0.5f;
                    dust.position = Projectile.Center;
                    dust.position.X -= num588;
                    dust.position.Y -= num589;
                    dust.velocity *= 0.2f;
                    num3 = num587;
                }
                if (Main.rand.NextBool(4))
                {
                    int num592 = 6;
                    int num593 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num592, Projectile.position.Y + (float)num592), Projectile.width - num592 * 2, Projectile.height - num592 * 2, 170, 0f, 0f, 75, default, 0.65f);
                    Dust dust = Main.dust[num593];
                    dust.velocity *= 0.5f;
                    dust.velocity += Projectile.velocity * 0.5f;
                }
                num3 = num586;
            }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.velocity = Projectile.oldVelocity * 0.2f;
            int num3;
            for (int num362 = 0; num362 < 100; num362 = num3 + 1)
            {
                int num363 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 170, 0f, 0f, 75, default, 1.2f);
                Dust dust = Main.dust[num363];
                if (Main.rand.NextBool(2))
                {
                    dust.alpha += 25;
                }
                if (Main.rand.NextBool(2))
                {
                    dust.alpha += 25;
                }
                if (Main.rand.NextBool(2))
                {
                    dust.alpha += 25;
                }
                if (Main.rand.NextBool(2))
                {
                    dust.scale = 0.6f;
                }
                else
                {
                    dust.noGravity = true;
                }
                dust.velocity *= 0.3f;
                dust.velocity += Projectile.velocity;
                dust.velocity *= 1f + (float)Main.rand.Next(-100, 101) * 0.01f;
                dust.velocity.X += (float)Main.rand.Next(-50, 51) * 0.015f;
                dust.velocity.Y += (float)Main.rand.Next(-50, 51) * 0.015f;
                dust.position = Projectile.Center;
                num3 = num362;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 480);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 480);
        }
    }
}

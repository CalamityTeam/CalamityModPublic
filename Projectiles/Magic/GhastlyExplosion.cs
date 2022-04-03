using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            int num3;
            int num328 = (int)Projectile.ai[0];
            for (int num329 = 0; num329 < 3; num329 = num3 + 1)
            {
                int num330 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, Projectile.velocity.X, Projectile.velocity.Y, num328, default, 1.2f);
                Main.dust[num330].position = (Main.dust[num330].position + Projectile.Center) / 2f;
                Main.dust[num330].noGravity = true;
                Dust dust = Main.dust[num330];
                dust.velocity *= 0.5f;
                num3 = num329;
            }
            for (int num331 = 0; num331 < 2; num331 = num3 + 1)
            {
                int num330 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, Projectile.velocity.X, Projectile.velocity.Y, num328, default, 0.4f);
                if (num331 == 0)
                {
                    Main.dust[num330].position = (Main.dust[num330].position + Projectile.Center * 5f) / 6f;
                }
                else if (num331 == 1)
                {
                    Main.dust[num330].position = (Main.dust[num330].position + (Projectile.Center + Projectile.velocity / 2f) * 5f) / 6f;
                }
                Dust dust = Main.dust[num330];
                dust.velocity *= 0.1f;
                Main.dust[num330].noGravity = true;
                Main.dust[num330].fadeIn = 1f;
                num3 = num331;
            }
            return;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            int num3;
            for (int num116 = 0; num116 < 20; num116 = num3 + 1)
            {
                int num117 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[num117].scale = 1.2f + (float)Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[num117].noGravity = true;
                dust = Main.dust[num117];
                dust.velocity *= 2.5f;
                dust = Main.dust[num117];
                dust.velocity -= Projectile.oldVelocity / 10f;
                num3 = num116;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                int num118 = 3;
                for (int num119 = 0; num119 < num118; num119 = num3 + 1)
                {
                    Vector2 vector8 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vector8.X == 0f && vector8.Y == 0f)
                    {
                        vector8 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vector8.Normalize();
                    vector8 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.oldPosition.X + (float)(Projectile.width / 2), Projectile.oldPosition.Y + (float)(Projectile.height / 2), vector8.X, vector8.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)((double)Projectile.damage * 0.8), Projectile.knockBack * 0.8f, Projectile.owner, Projectile.ai[0], 0f);
                    num3 = num119;
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class ThunderstormShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shot");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 25;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 9f)
            {
                for (int num121 = 0; num121 < 5; num121++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= Projectile.velocity / 5f * (float)num121;
                    dust.noGravity = true;
                    dust.noLight = true;
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    dust2.velocity = Vector2.Zero;
                    dust2.position -= Projectile.velocity / 5f * (float)num121;
                    dust2.noGravity = true;
                    dust2.noLight = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            var source = Projectile.GetSource_FromThis();
            SoundEngine.PlaySound(SoundID.Item125, Projectile.Center);
            for (int n = 0; n < 5; n++)
            {
                CalamityUtils.ProjectileRain(source, Projectile.Center, 200f, 100f, 1500f, 1500f, 29f, ModContent.ProjectileType<ThunderstormShotSplit>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 36);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 1f; //0.75
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 229, vector7.X, vector7.Y, 100, default, 1.2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 107, vector7.X, vector7.Y, 100, default, 1.2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }
    }
}

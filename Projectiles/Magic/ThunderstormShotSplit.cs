using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class ThunderstormShotSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shot");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 15;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 420)
            {
                Projectile.tileCollide = true;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 6f)
            {
                for (int num121 = 0; num121 < 5; num121++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 0.5f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= Projectile.velocity / 5f * (float)num121;
                    dust.noGravity = true;
                    dust.noLight = true;
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 0.5f)];
                    dust2.velocity = Vector2.Zero;
                    dust2.position -= Projectile.velocity / 5f * (float)num121;
                    dust2.noGravity = true;
                    dust2.noLight = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item125, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 16;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f; //0.75
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 229, vector7.X, vector7.Y, 100, default, 0.6f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 107, vector7.X, vector7.Y, 100, default, 0.6f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            Projectile.Damage();
        }
    }
}

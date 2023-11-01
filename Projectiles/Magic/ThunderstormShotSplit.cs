using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class ThunderstormShotSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

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
            Projectile.MaxUpdates = 20;
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
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 0.5f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= Projectile.velocity / 5f * (float)i;
                    dust.noGravity = true;
                    dust.noLight = true;
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 0.5f)];
                    dust2.velocity = Vector2.Zero;
                    dust2.position -= Projectile.velocity / 5f * (float)i;
                    dust2.noGravity = true;
                    dust2.noLight = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item125, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 16;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int dustAmt = 36;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f; //0.75
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, 229, dustDirection.X, dustDirection.Y, 100, default, 0.6f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, 107, dustDirection.X, dustDirection.Y, 100, default, 0.6f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            Projectile.Damage();
        }
    }
}

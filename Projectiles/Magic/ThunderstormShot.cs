using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class ThunderstormShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.MaxUpdates = 30;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 9f)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust greenDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    greenDust.velocity = Vector2.Zero;
                    greenDust.position -= Projectile.velocity / 5f * (float)i;
                    greenDust.noGravity = true;
                    greenDust.noLight = true;
                    Dust lightningDust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    lightningDust.velocity = Vector2.Zero;
                    lightningDust.position -= Projectile.velocity / 5f * (float)i;
                    lightningDust.noGravity = true;
                    lightningDust.noLight = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            var source = Projectile.GetSource_FromThis();
            SoundEngine.PlaySound(SoundID.Item125, Projectile.Center);
            for (int n = 0; n < 5; n++)
            {
                CalamityUtils.ProjectileRain(source, Projectile.Center, 200f, 100f, 1500f, 1500f, 29f, ModContent.ProjectileType<ThunderstormShotSplit>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.ExpandHitboxBy(36);
            int dustAmt = 36;
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 1f; //0.75
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, 229, dustDirection.X, dustDirection.Y, 100, default, 1.2f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            for (int j = 0; j < dustAmt; j++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotate = dustRotate.RotatedBy((double)((float)(j - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killDust = Dust.NewDust(dustRotate + dustDirection, 0, 0, 107, dustDirection.X, dustDirection.Y, 100, default, 1.2f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].noLight = true;
                Main.dust[killDust].velocity = dustDirection;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class EntropicFlechetteSmall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 rotateVector = new Vector2(6f, 12f);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustRotate = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + (float)i * 3.14159274f), default) * rotateVector;
                    int darkDust = Dust.NewDust(Projectile.Center, 0, 0, 27, 0f, 0f, 160, default, 1f);
                    Main.dust[darkDust].scale = 1f;
                    Main.dust[darkDust].noGravity = true;
                    Main.dust[darkDust].position = Projectile.Center + dustRotate;
                    Main.dust[darkDust].velocity = Projectile.velocity;
                }
            }

            int darkestDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 27, 0f, 0f, 100, default, 0.8f);
            Main.dust[darkestDust].noGravity = true;
            Main.dust[darkestDust].velocity *= 0f;

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}

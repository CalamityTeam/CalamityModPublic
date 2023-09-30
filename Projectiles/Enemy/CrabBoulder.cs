using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class CrabBoulder : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.timeLeft = 480;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0]++ < 30f)
            {
                Projectile.scale = MathHelper.Lerp(0.004f, 1f, Projectile.ai[0] / 30f);
            }
            else
            {
                if (Projectile.velocity.Y < 12f)
                    Projectile.velocity.Y += 0.18f;
            }
            Projectile.tileCollide = Projectile.ai[0] > 70f;
            Projectile.rotation += Math.Sign(Projectile.velocity.X) * 0.08f;
        }
        public override void OnKill(int timeLeft)
        {
            Utils.PoofOfSmoke(Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

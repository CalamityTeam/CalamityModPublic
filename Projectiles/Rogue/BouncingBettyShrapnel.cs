using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingBettyShrapnel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            if (Math.Abs(Projectile.position.Y - Projectile.oldPosition.Y) > 4f)
            {
                Projectile.velocity.X = 0f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.rotation = MathHelper.Pi;
            }
            Projectile.velocity.Y += 0.2f;
        }
    }
}

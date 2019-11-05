using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingBettyShrapnel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
            Main.projFrames[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.Calamity().rogue = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            if (projectile.velocity != Vector2.Zero)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                projectile.rotation = MathHelper.PiOver2;
            }
            projectile.velocity.Y += 0.2f;
        }
    }
}

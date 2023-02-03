using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GrimreaverBat : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grim Bat"); // There's an incredibly obvious thing that I could add/change in this name, but I will restrain
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 210;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();

            // Each bounce subtracts from projectile.ai[0] instead of penetration because it shouldn't pierce enemies
            if (Projectile.ai[0] <= 0)
            {
                Projectile.Kill();
            }

            // Fade out
            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 4;
            }

            // Current bat burst code occasionally has a few duds, so they get an acceleration boost that blends them in (also mildly interesting to look at)
            if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10)
            {
                Projectile.velocity *= 1.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.ai[0]--;
            return false;
        }
    }
}

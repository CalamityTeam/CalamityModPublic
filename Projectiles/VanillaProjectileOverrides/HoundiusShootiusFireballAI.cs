using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class HoundiusShootiusFireballAI
    {
        public static bool DoHoundiusShootiusFireballAI(Projectile projectile)
        {
            float enemyDistanceDetection = 1200f;
            float projVelocity = 12.5f; // Same as Vanilla.
            float rateOfChange = .2f;

            Player owner = Main.player[projectile.owner];
            NPC target = projectile.Center.MinionHoming(enemyDistanceDetection, owner);
            
            if (target is not null)
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(target.Center) * projVelocity, rateOfChange);
            
            // Yes, return true, the projectile will have all it's normal AI.
            // But will have this piece of homing code on top of it.
            // So it'll have all it's animation code, dust code and all that with the homing I added.
            return true;
        }
    }
}
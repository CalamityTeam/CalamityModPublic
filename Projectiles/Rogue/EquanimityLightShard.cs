using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class EquanimityLightShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
            projectile.penetrate = 3;
            projectile.tileCollide = true;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            projectile.rotation += 0.4f * projectile.direction;

            if (projectile.timeLeft < 51)
            {
                projectile.alpha += 5;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.timeLeft < 130)
            {
                return null;
            }
            else
            {
                return false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return true;
        }
    }
}

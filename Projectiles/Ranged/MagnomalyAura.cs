using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MagnomalyAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int radius = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnomaly Aura");
        }

        public override void SetDefaults()
        {
            projectile.ranged = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile parent = Main.projectile[0];
            bool active = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.identity == projectile.ai[0] && p.active && p.type == ModContent.ProjectileType<MagnomalyRocket>())
                {
                    parent = p;
                    active = true;
                }
            }

            if (active)
            {
                projectile.Center = parent.Center;
            }
            else
            {
                projectile.Kill();
            }

            if (!parent.active)
            {
                projectile.Kill();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}

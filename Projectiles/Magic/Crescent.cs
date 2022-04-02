using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Magic
{
    public class Crescent : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crescent Cutter");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.timeLeft = 200;
            projectile.aiStyle = 0;
            projectile.width = 20;
            projectile.height = 20;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.knockBack = 0;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.magic = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(230, 230, 255, projectile.alpha);
        }

        public override void AI()
        {
            Projectile parent = Main.projectile[(int)projectile.ai[0]];
            if (!parent.active) 
                projectile.Kill();

            projectile.rotation += 0.7f;

            projectile.ai[1]++;

            if (projectile.ai[1] > 20)
            {
                projectile.velocity += projectile.SafeDirectionTo(parent.Center) * 2.5f;
                if (projectile.WithinRange(parent.Center, 75f))
                    projectile.Kill();
            }
        }
    }
}
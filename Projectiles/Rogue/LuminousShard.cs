using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LuminousShard : ModProjectile
    {
        bool gravity = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 90 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            if (projectile.ai[0] == 0f && projectile.velocity.X == 0f && projectile.velocity.Y == -2f)
                gravity = true;

            projectile.ai[0] = 1f;
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            if (gravity)
                projectile.velocity.Y *= 1.05f;

            if (projectile.timeLeft < 90)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 10f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 176, projectile.oldVelocity.X / 4, projectile.oldVelocity.Y / 4, 0, default, 0.75f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 177, projectile.oldVelocity.X / 4, projectile.oldVelocity.Y / 4, 0, default, 0.75f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
            }
        }
    }
}

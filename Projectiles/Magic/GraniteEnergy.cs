using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GraniteEnergy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.magic = true;
            projectile.timeLeft = 90;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 60 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.ToRadians(90);
            if (Main.rand.NextBool(2))
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 100, default, 0.6f);

            if (projectile.timeLeft < 60)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 600f, 12f, 20f);
        }
    }
}

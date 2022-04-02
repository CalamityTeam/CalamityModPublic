using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class Phantom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.01f;

            Lighting.AddLight(projectile.Center, 0.2f, 0.2f, 0.2f);

            for (int num457 = 0; num457 < 2; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default, 1f);
                Main.dust[num458].noGravity = true;
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 12f, 20f);
        }
    }
}

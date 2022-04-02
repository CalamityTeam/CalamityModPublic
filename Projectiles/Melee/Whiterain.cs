using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Whiterain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item125, projectile.Center);
            }

            Lighting.AddLight(projectile.Center, 0.2f, 0.2f, 0.2f);

            for (int num457 = 0; num457 < 2; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 91, 0f, 0f, 100, default, 1.25f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.5f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
        }
    }
}

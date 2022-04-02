using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class HydrothermalSmoke : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smoke");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 6;
        }

        public override void AI()
        {
            if (projectile.timeLeft == 6)
                projectile.Center = Main.player[projectile.owner].Center;

            int randomDust = Main.rand.Next(4);
            if (randomDust == 3)
            {
                randomDust = 16;
            }
            else
            {
                randomDust = 127;
            }
            if (Main.rand.NextBool(4))
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDust, 0f, 0f, 100, default, 1f);
                if (Main.rand.NextBool(4))
                {
                    Main.dust[num469].scale *= 0.35f;
                }
                Main.dust[num469].velocity *= 0f;
            }

            Vector2 goreVec = new Vector2(projectile.position.X, projectile.position.Y);
            if (Main.rand.NextBool(8))
            {
                int smoke = Gore.NewGore(goreVec, default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}

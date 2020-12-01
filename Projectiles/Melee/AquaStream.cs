using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class AquaStream : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aqua Stream");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.alpha = 150;
            projectile.penetrate = 1;
            projectile.timeLeft = 14;
            projectile.melee = true;
        }

        public override void AI()
        {
            //projectile.ai[0] is the X increment while projectile.ai[1] is the Y increment
            projectile.velocity.X += projectile.ai[0];
            projectile.velocity.Y += projectile.ai[1];
            if (Main.rand.NextFloat() < 1f)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = projectile.Center;
                dust = Terraria.Dust.NewDustPerfect(position, 33, new Vector2(0f, 0f), 0, new Color(0, 142, 255), 2.368421f);
                dust.noGravity = true;
            }
        }
    }
}

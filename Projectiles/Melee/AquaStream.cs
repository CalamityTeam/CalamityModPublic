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
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.alpha = 150;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 14;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            //projectile.ai[0] is the X increment while projectile.ai[1] is the Y increment
            Projectile.velocity.X += Projectile.ai[0];
            Projectile.velocity.Y += Projectile.ai[1];
            if (Main.rand.NextFloat() < 1f)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = Projectile.Center;
                dust = Dust.NewDustPerfect(position, 33, new Vector2(0f, 0f), 0, new Color(0, 142, 255), 2.368421f);
                dust.noGravity = true;
            }
        }
    }
}

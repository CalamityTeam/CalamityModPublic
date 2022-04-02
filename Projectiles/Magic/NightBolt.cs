using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NightBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 20;
            projectile.friendly = true;
            projectile.timeLeft = 30;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            for (int num447 = 0; num447 < 2; num447++)
            {
                Vector2 vector33 = projectile.position;
                vector33 -= projectile.velocity * ((float)num447 * 0.25f);
                int num448 = Dust.NewDust(vector33, 1, 1, 27, 0f, 0f, 0, default, 1.25f);
                Main.dust[num448].position = vector33;
                Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                Main.dust[num448].velocity *= 0.1f;
            }
        }
    }
}

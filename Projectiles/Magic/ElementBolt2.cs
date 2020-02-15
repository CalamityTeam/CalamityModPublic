using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ElementBolt2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 100;
            projectile.friendly = true;
            projectile.timeLeft = 15;
            projectile.magic = true;
        }

		public override void AI()
		{
			int num448 = Dust.NewDust(projectile.position, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
			Main.dust[num448].noGravity = true;
			Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
		}
    }
}

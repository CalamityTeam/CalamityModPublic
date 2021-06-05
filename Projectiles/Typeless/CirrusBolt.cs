using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class CirrusBolt : ModProjectile
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
            projectile.extraUpdates = 100;
            projectile.friendly = true;
            projectile.timeLeft = 45;
            projectile.magic = true;
			projectile.npcProj = true;
		}

		public override void AI()
		{
			if (projectile.timeLeft % 2f == 0f)
			{
				Vector2 vector33 = projectile.position;
				vector33 -= projectile.velocity * 0.25f;
				int num448 = Dust.NewDust(vector33, 1, 1, 234, 0f, 0f, 0, default, 1.25f);
				Main.dust[num448].position = vector33;
				Main.dust[num448].noGravity = true;
				Main.dust[num448].noLight = true;
				Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.013f;
				Main.dust[num448].velocity *= 0.1f;
			}
		}
    }
}

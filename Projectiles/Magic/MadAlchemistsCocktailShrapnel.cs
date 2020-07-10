using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class MadAlchemistsCocktailShrapnel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Vector2 adjustVec = new Vector2(6f, 12f);
			for (int i = 0; i < 2; i++)
			{
				Vector2 offset = Vector2.UnitX * -15f;
				offset = -Vector2.UnitY.RotatedBy((double)(projectile.localAI[0] * 0.1308997f + i * MathHelper.Pi), default) * adjustVec * 0.75f;
				int shrapnel = Dust.NewDust(projectile.Center, 0, 0, 173, 0f, 0f, 160, default, 0.75f);
				Main.dust[shrapnel].noGravity = true;
				Main.dust[shrapnel].position = projectile.Center + offset;
				Main.dust[shrapnel].velocity = projectile.velocity;
			}
            int violet = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1f);
            Main.dust[violet].noGravity = true;

			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 500f, 12f, 20f);
        }
    }
}

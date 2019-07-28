using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MarniteObliterator : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Obliterator");
		}

		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 22;
			projectile.aiStyle = 20;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.hide = true;
			projectile.ownerHitCheck = true;
			projectile.melee = true;
		}
	}
}

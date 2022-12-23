using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterProjectileSlotHolder : ModProjectile //The sole purpose of this projectile is to hold a slot earlier in the projectile array than AquasScepterCloud so that the projectile AquasScepterCloudFlash can be drawn above AquasScepterCloud for the purpose of properply displaying the flash animation 99.9% of the time.
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Projectile Slot Holder");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;

		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = 0; 
			Projectile.timeLeft = 1;
			Projectile.alpha = 255;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterCloudFlash : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Storm Cloud Flash");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;
		}
		
		public sealed override void SetDefaults() {
			Projectile.width = 252;
			Projectile.height = 78;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 27;
			Projectile.penetrate = -1; 
			Projectile.alpha = 125;
		}

		public override bool? CanCutTiles() {
			return false;
		}

		public override void AI() {
			Projectile.alpha += 5;
		}
	}
}

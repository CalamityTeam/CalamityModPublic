using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterTeslaAura : ModProjectile //THIS IS ONLY THE VISUALS OF THE AURA. YOU ALSO NEED TO SPAWN 'AquasScepterTeslaAuraHitbox'
	{
		public int UpdateCounter; //int that is incremented every loop of AI. This is used for sprite animation.
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Tesla Aura");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 5;
		}

		public sealed override void SetDefaults() {
			Projectile.width = 216;
			Projectile.height = 216;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1; 
			Projectile.timeLeft = 27;
			Projectile.alpha = 0;
		}

		public override bool? CanCutTiles() {
			return false;
		}

        public override void AI() { 
			Projectile.scale = 3f;
			UpdateCounter++;
			Projectile.alpha += 10;

			if (UpdateCounter%4==0) // Goes to the next frame of cloud animation every 4 frames. Doesn't need to be reset, because the projectile becomes invisible by update 26 via Projectile.alpha
            {
				Projectile.frameCounter++;
				Projectile.frame++;
            }
		}
	}
}

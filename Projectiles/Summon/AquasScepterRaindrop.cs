using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterRaindrop : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Rain Droplet");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
        }

		public sealed override void SetDefaults() //If you want to change the damage of this projectile, just change the base damage of AquasScepter, the item. Note that this will affect the damage of AquasScepterTeslaAuraHitbox as well, because the damage of said projectile is set as a multiple of the base daamge at the bottom of AquasScepterCloud.cs
        {
			Projectile.width = 20; //The sprite for the raindrop deliberately has invisible pixels added on each side of it to increase the width of the hitbox easily
			Projectile.height = 48;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = true; 
			Projectile.DamageType = DamageClass.Summon; 
			Projectile.penetrate = -1; 
			Projectile.timeLeft = 120;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 25;
		}

		public override bool? CanCutTiles() {
			return false;
		}
		public override bool MinionContactDamage() {
			return true;
		}
	}
}

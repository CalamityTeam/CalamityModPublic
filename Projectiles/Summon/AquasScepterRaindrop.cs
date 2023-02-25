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
        }

		public sealed override void SetDefaults() //If you want to change the damage of this projectile, just change the base damage of AquasScepter, the item. Note that this will affect the damage of AquasScepterTeslaAura as well, because the damage of said projectile is set as a multiple of the base damage at the bottom of AquasScepterCloud.cs
        {
			Projectile.width = 20; //The sprite for the raindrop deliberately has invisible pixels added on each side of it to increase the width of the hitbox easily
			Projectile.height = 48;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = true; 
			Projectile.DamageType = DamageClass.Summon; 
			Projectile.penetrate = 2; 
			Projectile.timeLeft = 120;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
		}

		public override bool? CanCutTiles() {
			return false;
		}
		public override bool MinionContactDamage() {
			return true;
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6f);
        }
        public override void AI()
        {
            if (Projectile.timeLeft <= 8)
            {
                Projectile.Opacity -= 0.125f;
                Projectile.velocity *= 0.92f;
            }
        }
    }
}

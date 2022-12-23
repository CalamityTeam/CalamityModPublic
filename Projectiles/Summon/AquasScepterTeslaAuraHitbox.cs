using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterTeslaAuraHitbox : ModProjectile
	{
        private static float TeslaAuraRadius = 288.0f;
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tesla Aura");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

		}

		public sealed override void SetDefaults() //If you want to change the damage of this projectile, look near the bottom of AquasScepterCloud.cs because this projectile's damage is set in Projectile.NewProjectile as a multiple of the AquasScepterCloud damage value
		{
			Projectile.width = 480;
			Projectile.height = 480;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = true; 
			Projectile.DamageType = DamageClass.Summon; 
			Projectile.penetrate = -1; 
			Projectile.timeLeft = 5;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 6;
			Projectile.alpha = 255;
		}

		public override bool? CanCutTiles()
		{
			return true;
		}

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, TeslaAuraRadius, targetHitbox);

	}
}

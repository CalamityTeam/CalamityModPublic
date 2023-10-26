using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterTeslaAura : ModProjectile, ILocalizedModType
	{
        public new string LocalizationCategory => "Projectiles.Summon";
        static float TeslaAuraScale = 3f; //Changing this float will change the radius of the hitbox and scale of the sprite at the same time, making the visual match the hitbox no matter what size is chosen.
        public bool ableToHit = true; // bool that controls the state of CanDamage()
		public override void SetStaticDefaults() {
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 5;
		}

		public sealed override void SetDefaults() //If you want to change the damage of this projectile, look near the bottom of AquasScepterCloud.cs because this projectile's damage is set in Projectile.NewProjectile as a multiple of the AquasScepterCloud damage value
        {
			Projectile.width = 216;
			Projectile.height = 216;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1; 
			Projectile.timeLeft = 27;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 28;
            Projectile.alpha = 0;
            Projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;
		}

        public override bool? CanDamage() => ableToHit ? (bool?)null : false;
        public override Color? GetAlpha(Color lightColor) => Color.White * (1 - (Projectile.alpha / 255f));

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, (TeslaAuraScale * 96.0f), targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6f);
        }

        public override void AI() { 
			Projectile.scale = TeslaAuraScale;
			Projectile.alpha += 11;
            Projectile.ai[0]++;

            Projectile.frameCounter++;
            Projectile.frame = (int)(Projectile.frameCounter / 5.4f);

            if (Projectile.ai[0] > 8f) //After the Tesla Aura has been alive for 8 frames, disables it's ability to deal damage. This is done so that there is some leniency on allowing new enemies to enter the damaging range for a bit to make the sentry feel better to use.
            {
                ableToHit = false;
            }
        }
	}
}

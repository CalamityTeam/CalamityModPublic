using Microsoft.Xna.Framework; //God forbid someone has to read all the code relating to this weapon in the future. I salute you. -AquaSG
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterCloud : ModProjectile
	{
		public int LightningTimer; //int that is incremented every loop of AI. This is used for the cooldown of the tesla aura attack.
        public int RainTimer;      //int that is incremented every loop of AI. This is used for the cooldown between raindrops.
        public int UpdateCounter;  //int that is incremented every loop of AI. This is used for sprite animation.
        public override void SetStaticDefaults() {
			DisplayName.SetDefault("Storm Cloud");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 5;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
		}

		public sealed override void SetDefaults() {
			Projectile.width = 252;
			Projectile.height = 78;
			Projectile.tileCollide = false;
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.friendly = true; 
			Projectile.sentry = true; 
			Projectile.DamageType = DamageClass.Summon; 
			Projectile.minionSlots = 1f; 
			Projectile.penetrate = -1; 
			Projectile.light = 1f;
		}

		public override bool? CanCutTiles() {
			return false;
		}

		public override bool MinionContactDamage() {
			return false;
		}

		public override Color? GetAlpha(Color drawColor)
		{
			return Color.White;
		}

		public override void AI() {
			Player owner = Main.player[Projectile.owner];

			float distanceFromTarget = 700f;
			var targetCenter = Projectile.position;
			bool foundTarget = false;

			LightningTimer++;
			RainTimer++;
			UpdateCounter++;

			if (UpdateCounter % 6 == 0) // Goes to the next frame of cloud animation every 6 frames
                {
					Projectile.frameCounter++;
					Projectile.frame++;
                }
			if (Projectile.frameCounter >= 5) // Resets the spritesheet to the beginning once it reaches the final frame
                {
					Projectile.frameCounter = 0;
					Projectile.frame = 0;
					UpdateCounter = 0;
                }
			

			if (RainTimer >= 3) // Spawns a raindrop every 3 frames, displaced down and randomly along the length of the cloud
                {
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), (Projectile.Center.X + Main.rand.Next(-110, 111)), (Projectile.Center.Y + 44f), 0f, 15f, ModContent.ProjectileType<AquasScepterRaindrop>(), Projectile.damage, 0, Projectile.owner);

					RainTimer = 0;
				}

			if (!foundTarget) {
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;

						if ((closest && inRange) || !foundTarget) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			if (foundTarget) {
				if (distanceFromTarget < 272f) {
					if (LightningTimer >= 60) { //Every  60 AI cycles, plays the lightning sound and spawns 6 projectiles: 3 projectile slot holders, The tesla aura visual, the tesla aura hitbox, and the projectile that simulates the cloud changing brightness.
						SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/LightningAura"));
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterProjectileSlotHolder>(), 0, 0, Projectile.owner); //Spawning 3 projectiles for 1 frame each to make sure that the latter 3 projectiles are rendered on top of the main storm cloud
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterProjectileSlotHolder>(), 0, 0, Projectile.owner); //Without these 3 projectiles here, some of the projectiles (more importantly, the cloud flash) are rendered below the cloud in some situations. This prevents 99% of said situations. It should be fine.
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterProjectileSlotHolder>(), 0, 0, Projectile.owner);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterTeslaAuraHitbox>(), (Projectile.damage*20), 16, Projectile.owner); // Uses a different projectile for the hitbox because I don't want anything to do with Projectile.scale fuckery
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterTeslaAura>(), 0, 0, Projectile.owner);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterCloudFlash>(), 0, 0, Projectile.owner);
						LightningTimer = 0;
					}
				}
			}
		}
	}
}

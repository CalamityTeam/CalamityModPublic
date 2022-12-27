using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class AquasScepterCloud : ModProjectile
	{
        public ref float LightningTimer => ref Projectile.ai[0];
        public ref float RainTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults() {
			DisplayName.SetDefault("Storm Cloud");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 5;
		}

		public sealed override void SetDefaults() {
			Projectile.width = 252;
			Projectile.height = 78;
            Projectile.hide = true;
            Projectile.tileCollide = false;
			Projectile.timeLeft = Projectile.SentryLifeTime;
			Projectile.friendly = true; 
			Projectile.sentry = true; 
			Projectile.DamageType = DamageClass.Summon; 
			Projectile.penetrate = -1; 
			Projectile.light = 1f;
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            behindNPCs.Add(index);
        }

        public override bool? CanDamage() => false;

        public override Color? GetAlpha(Color drawColor)
		{
			return Color.White;
		}

		public override void AI() {
			float distanceFromTarget = 700f;
			var targetCenter = Projectile.position;
			bool foundTarget = false;

			LightningTimer++;
			RainTimer++;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

			if (RainTimer >= 3f) // Spawns a raindrop every 3 frames, displaced down and randomly along the length of the cloud
                {
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), (Projectile.Center.X + Main.rand.Next(-110, 111)), (Projectile.Center.Y + 57f), 0f, 15f, ModContent.ProjectileType<AquasScepterRaindrop>(), Projectile.damage, 0, Projectile.owner);

					RainTimer = 0f;
				}


            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;

                    if ((closest && inRange) || !foundTarget)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget) {
				if (distanceFromTarget < 300) {
					if (LightningTimer >= 60f) { //Every  60 AI cycles, plays the lightning sound and spawns 2 projectiles: the tesla aura for dealing damage in an aoe, and the cloud flash to simulate the brightness of the main cloud changing.
						SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/LightningAura"));
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterTeslaAura>(), (Projectile.damage*20), 16, Projectile.owner);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<AquasScepterCloudFlash>(), 0, 0, Projectile.owner);
						LightningTimer = 0f;
					}
				}
			}
		}
	}
}

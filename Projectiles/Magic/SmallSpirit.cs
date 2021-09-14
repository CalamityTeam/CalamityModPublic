using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SmallSpirit : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public Projectile ProjectileOwner
		{
			get
			{
                int spiritType = ModContent.ProjectileType<SpiritCongregation>();
                for (int i = 0; i < Main.maxProjectiles; i++)
				{
                    if (Main.projectile[i].type != spiritType || Main.projectile[i].identity != projectile.ai[0] || 
                        Main.projectile[i].owner != projectile.owner)
                    {
                        continue;
                    }

                    return Main.projectile[i];
				}
                return null;
			}
		}
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Angry Spirit");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 18;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
        }

        public override void AI()
        {
            // Handle frames.
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];

            float maxOpacity = 1f;

            // Fly around towards a target.
            Entity target = Owner;
            float flySpeed = 8f;
            float flyInertia = 54f;

            projectile.hostile = true;
            projectile.friendly = false;
            if (ProjectileOwner != null && (ProjectileOwner.ModProjectile<SpiritCongregation>().CurrentPower > 0.97f || projectile.timeLeft < 95))
            {
                projectile.hostile = false;

                target = ProjectileOwner;
                flySpeed = 29f;
                flyInertia = 5f;

                // Die if close to the owner projectile, effectively being absorbed.
                projectile.Center = projectile.Center.MoveTowards(target.Center, 20f);
                if (projectile.WithinRange(target.Center, 84f))
                    projectile.Kill();

                // Become translucent when returning to the projectile owner.
                maxOpacity = 0.4f;
            }

            // Die if no valid target exists or the projectile owner is absent.
            if (target is null || ProjectileOwner is null || !ProjectileOwner.active)
			{
                projectile.Kill();
                return;
			}

            // If not close to the target, redirect towards them.
            if (!projectile.WithinRange(target.Center, 260f))
            {
                Vector2 idealVelocity = projectile.SafeDirectionTo(target.Center) * flySpeed;
                projectile.velocity = (projectile.velocity * (flyInertia - 1f) + idealVelocity) / flyInertia;
            }

            // Otherwise accelerate.
            else
                projectile.velocity *= 1.01f;

            // Rapidly fade in.
            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.075f, 0f, maxOpacity);

            // Decide rotation.
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

		public override void Kill(int timeLeft)
		{
            for (int i = 0; i < 6; i++)
			{
                Dust phantoplasm = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(12f, 12f), 261);
                phantoplasm.color = Color.Lerp(Color.LightPink, Color.Red, Main.rand.NextFloat(0.67f));
                phantoplasm.scale = 1.2f;
                phantoplasm.fadeIn = 0.55f;
                phantoplasm.noGravity = true;
            }
		}
	}
}

using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
	public class LittleLightProj : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];

        public Color LightColor => new Color(160, 251, 255);
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Little Light");
            Main.projFrames[projectile.type] = 9;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 24;
            projectile.friendly = true;
            projectile.netImportant = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (VerifyOwnerIsPresent())
                return;

            HandleFrames();
            DoSpinEffect();
            HoverTowardsOwnersShoulder();

            // Look in the same direction as the projectile's owner.
            projectile.spriteDirection = Owner.direction;

            // Emit light.
            // Suspicious Looking Tentacle uses 1.5f as a brightness factor, for reference.
            Lighting.AddLight(projectile.Center, LightColor.ToVector3() * 2.5f);

            Time++;
        }

        public bool VerifyOwnerIsPresent()
        {
            // No logic should be run if the player is no longer active in the game.
            if (!Owner.active)
            {
                projectile.Kill();
                return true;
            }

            if (Owner.dead)
                Owner.Calamity().littleLightPet = false;
            if (Owner.Calamity().littleLightPet)
                projectile.timeLeft = 2;

            return false;
        }

        public void HandleFrames()
		{
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];
		}

        public void DoSpinEffect()
		{
            // Spin around from time to time.
            if (projectile.frameCounter % 180f > 150f)
                projectile.rotation += MathHelper.TwoPi / 30f;
            else
                projectile.rotation = 0f;
        }

        public void HoverTowardsOwnersShoulder()
		{
            Vector2 destination = Owner.Top;
            destination.X -= Owner.direction * Owner.width * 1.25f;
            destination.Y += Owner.height * 0.25f;

            // Hover in the air a little bit over time so that the pet doesn't seem static.
            destination.Y += (float)Math.Sin(MathHelper.TwoPi * Time / 45f) * 5f;

            projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.125f);
            if (projectile.WithinRange(destination, 10f))
                projectile.Center = destination;
            else
                projectile.Center += projectile.SafeDirectionTo(destination) * 4f;

            projectile.Center = destination;
		}
	}
}

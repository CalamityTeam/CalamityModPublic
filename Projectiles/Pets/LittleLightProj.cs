using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class LittleLightProj : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public Color LightColor => new Color(160, 251, 255);
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Little Light");
            Main.projFrames[Projectile.type] = 9;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (VerifyOwnerIsPresent())
                return;

            HandleFrames();
            DoSpinEffect();
            HoverTowardsOwnersShoulder();

            // Look in the same direction as the projectile's owner.
            Projectile.spriteDirection = Owner.direction;

            // Emit light.
            // Suspicious Looking Tentacle uses 1.5f as a brightness factor, for reference.
            Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 2.5f);

            Time++;
        }

        public bool VerifyOwnerIsPresent()
        {
            // No logic should be run if the player is no longer active in the game.
            if (!Owner.active)
            {
                Projectile.Kill();
                return true;
            }

            if (Owner.dead)
                Owner.Calamity().littleLightPet = false;
            if (Owner.Calamity().littleLightPet)
                Projectile.timeLeft = 2;

            return false;
        }

        public void HandleFrames()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];
        }

        public void DoSpinEffect()
        {
            // Spin around from time to time.
            if (Projectile.frameCounter % 180f > 150f)
                Projectile.rotation += MathHelper.TwoPi / 30f;
            else
                Projectile.rotation = 0f;
        }

        public void HoverTowardsOwnersShoulder()
        {
            Vector2 destination = Owner.Top;
            destination.X -= Owner.direction * Owner.width * 1.25f;
            destination.Y += Owner.height * 0.25f;

            // Hover in the air a little bit over time so that the pet doesn't seem static.
            destination.Y += (float)Math.Sin(MathHelper.TwoPi * Time / 45f) * 5f;

            Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.125f);
            if (Projectile.WithinRange(destination, 10f))
                Projectile.Center = destination;
            else
                Projectile.Center += Projectile.SafeDirectionTo(destination) * 4f;

            Projectile.Center = destination;
        }
    }
}

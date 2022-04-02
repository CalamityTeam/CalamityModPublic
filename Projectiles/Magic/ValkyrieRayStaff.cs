using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ValkyrieRayStaff : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Magic/ValkyrieRay";

        private const float AimResponsiveness = 0.66f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valkyrie Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 52;
            // This projectile has no hitboxes and no damage type.
            projectile.friendly = false;
            projectile.melee = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 900;
        }

        // ai[0] is a time-dilated frame counter. ai[1] is whether the beam has already fired.
        // localAI[0] is the rate at which the "frame" counter increases.
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            // Calculate how quickly the staff should charge. Charge increases by some number close to 1 every frame.
            // Speed increasing reforges make this number greater than 1. Slowing reforges make it smaller than 1.
            if (projectile.localAI[0] == 0f)
                projectile.localAI[0] = 47f / player.ActiveItem().useTime;

            // Increment the timer for the staff. If the timer has passed 47, destroy it.
            projectile.ai[0] += projectile.localAI[0];
            int maxTime = ValkyrieRay.ChargeFrames + ValkyrieRay.CooldownFrames;
            if (projectile.ai[0] > maxTime)
            {
                projectile.Kill();
                return;
            }

            // Compute the weapon's charge.
            float chargeLevel = MathHelper.Clamp(projectile.ai[0] / ValkyrieRay.ChargeFrames, 0f, 1f);

            // Common code among holdouts to keep the holdout projectile directly in the player's hand
            UpdatePlayerVisuals(player, rrp);

            // Compute the gem position, which is needed for visual effects
            float angle = projectile.rotation - MathHelper.PiOver2;
            Vector2 gemOffset = Vector2.One * ValkyrieRay.GemDistance * 1.4142f; // distance to gem on staff
            Vector2 gemPos = projectile.Center + gemOffset.RotatedBy(angle);

            // Firing or charging?
            if (chargeLevel >= 1f && projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f; // so it never fires again
                FiringEffects(gemPos);
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile laser = Projectile.NewProjectileDirect(gemPos, projectile.velocity, ModContent.ProjectileType<ValkyrieRayBeam>(), projectile.damage, projectile.knockBack, projectile.owner);
                    laser.Center = gemPos;
                }
            }
            else if (projectile.ai[1] == 0f)
            {
                // The player can constantly re-aim the staff while it's charging, but once it fires it is locked in place.
                UpdateAim(rrp, projectile.velocity.Length());
                ChargingEffects(gemPos, chargeLevel);
            }
        }

        private void UpdatePlayerVisuals(Player player, Vector2 rrp)
        {
            // Place the projectile directly into the player's hand at all times
            projectile.Center = rrp;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // The staff is a holdout projectile, so change the player's variables to reflect that
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // Multiplying by projectile.direction is required due to vanilla spaghetti.
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }

        // Adjusts the aim vector of the staff to point towards the mouse. This is Last Prism code.
        private void UpdateAim(Vector2 source, float speed)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(projectile.velocity), AimResponsiveness));
            aimVector *= 30f;

            if (aimVector != projectile.velocity)
                projectile.netUpdate = true;
            projectile.velocity = aimVector;
        }

        private void ChargingEffects(Vector2 center, float chargeLevel)
        {
            Lighting.AddLight(center, ValkyrieRay.LightColor.ToVector3() * chargeLevel);

            int numDust = 2;
            int dustID = 73;
            float incomingRadius = 9f;
            for (int i = 0; i < numDust; ++i)
            {
                Vector2 offsetUnit = Main.rand.NextVector2Unit();
                Vector2 dustPos = center + offsetUnit * incomingRadius;
                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f);
                d.velocity = offsetUnit * -Main.rand.NextFloat(2f, 3.5f);
                d.scale = Main.rand.NextFloat(0.4f, 1f);
                d.noGravity = true;
            }
        }

        private void FiringEffects(Vector2 center)
        {
            Main.PlaySound(SoundID.Item28, center);
            Main.PlaySound(SoundID.Item60, center);
            int numDust = 36;
            int dustID = 73;
            for (int i = 0; i < numDust; ++i)
            {
                Dust d = Dust.NewDustDirect(center, 0, 0, dustID, 0f, 0f);
                d.velocity = (i * MathHelper.TwoPi / numDust).ToRotationVector2() * 2.2f;
                d.scale = 1.4f;
                d.noGravity = true;
            }
        }
    }
}

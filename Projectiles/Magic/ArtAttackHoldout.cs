using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ArtAttackHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public const float AimResponsiveness = 0.72f;

        public override string Texture => "CalamityMod/Items/Weapons/Magic/ArtAttack";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Art Attack");
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.friendly = false;
            projectile.magic = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 90000;
        }

        public override void AI()
        {
            // Common code among holdouts to keep the holdout projectile directly in the player's hand.
            UpdatePlayerVisuals();

            if (Main.myPlayer == projectile.owner)
            {
                // Update aim.
                UpdateAim();

                // Create the star.
                if (projectile.localAI[0] == 0f)
                {
                    Vector2 initialStarVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * 15f;
                    Projectile.NewProjectile(projectile.Center, initialStarVelocity, ModContent.ProjectileType<ArtAttackStar>(), projectile.damage, projectile.knockBack, projectile.owner);
                    projectile.localAI[0] = 1f;
                }

                if (!Owner.channel)
                    projectile.Kill();
            }
        }

        private void UpdatePlayerVisuals()
        {
            // Place the projectile directly into the player's hand at all times.
            projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // The staff is a holdout projectile; update the player's variables to reflect this.
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = CalamityUtils.WrapAngle90Degrees(projectile.rotation);
        }

        // Adjusts the aim vector of the staff to point towards the mouse. This is Last Prism code.
        private void UpdateAim()
        {
            Vector2 aimOffset = projectile.SafeDirectionTo(Main.MouseWorld, -Vector2.UnitY);
            aimOffset = Vector2.Lerp(aimOffset, Vector2.Normalize(projectile.velocity), AimResponsiveness).SafeNormalize(Vector2.UnitY) * 30f;

            if (aimOffset != projectile.velocity)
            {
                projectile.netSpam = 0;
                projectile.netUpdate = true;
            }
            projectile.velocity = aimOffset;
        }

        public override bool CanDamage() => false;
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ArtAttackHoldout : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public const float AimResponsiveness = 0.72f;

        public override string Texture => "CalamityMod/Items/Weapons/Magic/ArtAttack";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Art Attack");
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = false;
            // projectile.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90000;
        }

        public override void AI()
        {
            // Common code among holdouts to keep the holdout projectile directly in the player's hand.
            UpdatePlayerVisuals();

            if (Main.myPlayer == Projectile.owner)
            {
                // Update aim.
                UpdateAim();

                // Create the star.
                if (Projectile.localAI[0] == 0f)
                {
                    Vector2 initialStarVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, initialStarVelocity, ModContent.ProjectileType<ArtAttackStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.localAI[0] = 1f;
                }

                if (!Owner.channel)
                    Projectile.Kill();
            }
        }

        private void UpdatePlayerVisuals()
        {
            // Place the projectile directly into the player's hand at all times.
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // The staff is a holdout projectile; update the player's variables to reflect this.
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = CalamityUtils.WrapAngle90Degrees(Projectile.rotation);
        }

        // Adjusts the aim vector of the staff to point towards the mouse. This is Last Prism code.
        private void UpdateAim()
        {
            Vector2 aimOffset = Projectile.SafeDirectionTo(Main.MouseWorld, -Vector2.UnitY);
            aimOffset = Vector2.Lerp(aimOffset, Vector2.Normalize(Projectile.velocity), AimResponsiveness).SafeNormalize(Vector2.UnitY) * 30f;

            if (aimOffset != Projectile.velocity)
            {
                Projectile.netSpam = 0;
                Projectile.netUpdate = true;
            }
            Projectile.velocity = aimOffset;
        }

        public override bool? CanDamage() => false;
    }
}

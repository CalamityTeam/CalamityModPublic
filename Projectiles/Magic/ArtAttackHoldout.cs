using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ArtAttackHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ArtAttack>();
        public Player Owner => Main.player[Projectile.owner];
        public const float AimResponsiveness = 0.72f;

        public override string Texture => "CalamityMod/Items/Weapons/Magic/ArtAttack";

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = false;
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
                int attackType = ModContent.ProjectileType<ArtAttackStar>();
                if (Owner.ownedProjectileCounts[attackType] == 0)
                {
                    if (Projectile.ai[0] >= 0f && Owner.CheckMana(Owner.ActiveItem(), -1, true, false))
                    {
                        SoundEngine.PlaySound(ArtAttack.UseSound, Owner.Center);
                        Vector2 initialStarVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, initialStarVelocity, attackType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.ai[0] = -24f;
                    }
                    else
                        Projectile.ai[0]++;
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

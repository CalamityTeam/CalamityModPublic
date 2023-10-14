using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Ranged.Photoviscerator;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class PhotovisceratorHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Photoviscerator>();
        public Player Owner => Main.player[Projectile.owner];
        public bool OwnerCanShoot => (Owner.channel || Owner.Calamity().mouseRight) && !Owner.noItems && !Owner.CCed;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ForcedLifespan => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 170;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            UpdateProjectileHeldVariables();
            ManipulatePlayerVariables();

            // Despawn the projectile if the player can't shoot
            // Any forced lifespan (triggered by attacking right click to prevent click spamming) will have priority
            if (!OwnerCanShoot)
            {
                ForcedLifespan--;
                if (ForcedLifespan <= 0f)
                    Projectile.Kill();
                return;
            }

            // Don't try to perform attacks if it's not the holder
            if (Projectile.owner != Main.myPlayer)
                return;

            // Immediately killed if ammo is out            
            if (!Owner.HasAmmo(Owner.ActiveItem()))
            {
                Projectile.Kill();
                return;
            }

            // Right-click attacks
            if (Owner.Calamity().mouseRight && !Owner.channel)
            {
                // Can't shoot on frame 1 as it can't use ammo yet
                if (ShootTimer < 0f)
                    RightClickAttack();

                ShootTimer--;
            }
            else if (!Owner.Calamity().mouseRight && Owner.channel)
            {
                ShootTimer++;
                LeftClickAttack();
            }
        }

        public void LeftClickAttack()
        {
            // Consume ammo and retrieve projectile stats; has a chance to not consume ammo
            Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _, Main.rand.NextFloat() <= AmmoNotConsumeChance);

            var source = Projectile.GetSource_FromThis();
            Vector2 position = Projectile.Center + Projectile.velocity * 40f;
            Vector2 velocity = Projectile.velocity * shootSpeed;

            // Main fire stream
            SoundEngine.PlaySound(SoundID.Item34, Owner.MountedCenter);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<ExoFire>(), damage, knockback, Projectile.owner, Main.rand.NextFloat(0f, 3f));

            // Shoots light bombs every once in a while, rate of which equals to the item's use time
            if (ShootTimer >= Owner.ActiveItem().useTime)
            {
                ShootTimer = 0f;

                for (int i = 0; i < 2; i++)
                {
                    position += Projectile.velocity * 64f;
                    int yDirection = (i == 0).ToDirectionInt();
                    Vector2 bombVel = velocity.RotatedBy(0.2f * yDirection);

                    Projectile lightBomb = Projectile.NewProjectileDirect(source, position, bombVel, ProjectileType<ExoLight>(), damage, knockback, Projectile.owner);
                    lightBomb.localAI[1] = yDirection;
                    lightBomb.netUpdate = true;
                }
            }
        }

        public void RightClickAttack()
        {
            // Multiplied by the ratio of attack speed gained from modifiers
            ShootTimer = (RightClickCooldown * Owner.ActiveItem().useTime / (float)LightBombCooldown) - 1f;
            ForcedLifespan = ShootTimer;

            // Consume ammo and retrieve projectile stats
            Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _);

            var source = Projectile.GetSource_FromThis();
            Vector2 position = Projectile.Center + Projectile.velocity * 120f;
            Vector2 velocity = Projectile.velocity * shootSpeed * RightClickVelocityMult;

            SoundEngine.PlaySound(SoundID.Item34, Owner.MountedCenter);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<ExoFlareCluster>(), damage, knockback, Projectile.owner);
        }

        public void UpdateProjectileHeldVariables()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - Projectile.Size * 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        public void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;
    }
}

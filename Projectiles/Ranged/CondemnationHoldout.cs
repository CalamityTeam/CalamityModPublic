using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        public ref float ChargeUpTime => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Condemnation";
        public override void SetStaticDefaults() => DisplayName.SetDefault("Condemnation");

        public override void SetDefaults()
        {
            projectile.width = 130;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            Vector2 tipPosition = armPosition + projectile.velocity * projectile.width * 0.5f;
            if (ChargeUpTime < 20f)
                ReleaseInitialChargeDust(tipPosition);
            if (ChargeUpTime == 20f)
                ReleaseInitialBurstDust(tipPosition);

            // Release projectiles after done charging.
            if (!OwnerCanShoot)
            {
                projectile.Kill();

                // If killed prior to the charge being complete, don't bother spawning anything. Just die.
                if (ChargeUpTime < 20f)
                    return;

                int totalProjectilesToShoot = (int)Math.Pow(ChargeUpTime * 0.15f, 0.71) + 1;
                if (totalProjectilesToShoot > 32)
                    totalProjectilesToShoot = 32;

                for (int i = 0; i < totalProjectilesToShoot; i++)
                    ShootProjectiles(tipPosition, MathHelper.Lerp(0.425f, 1f, i / (float)totalProjectilesToShoot));

                Main.PlaySound(SoundID.DD2_BallistaTowerShot);
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
            ChargeUpTime++;
        }

        public void ReleaseInitialBurstDust(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 36; i++)
            {
                Dust chargeMagic = Dust.NewDustPerfect(tipPosition, 267);
                chargeMagic.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 5f + Owner.velocity;
                chargeMagic.scale = Main.rand.NextFloat(1f, 1.5f);
                chargeMagic.color = Color.Violet;
                chargeMagic.noGravity = true;
            }
        }

        public void ReleaseInitialChargeDust(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                Dust chargeMagic = Dust.NewDustPerfect(tipPosition + Main.rand.NextVector2Circular(20f, 20f), 267);
                chargeMagic.velocity = (tipPosition - chargeMagic.position) * 0.1f + Owner.velocity;
                chargeMagic.scale = Main.rand.NextFloat(1f, 1.5f);
                chargeMagic.color = projectile.GetAlpha(Color.White);
                chargeMagic.noGravity = true;
            }
        }

        public void ShootProjectiles(Vector2 tipPosition, float speedFactor)
		{
            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            int projectileType = ModContent.ProjectileType<CondemnationArrow>();
            float shootSpeed = heldItem.shootSpeed * speedFactor * 1.5f;
            int damage = (int)(Owner.GetWeaponDamage(heldItem) * 1.45);
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref damage, ref knockback, false);
            projectileType = ModContent.ProjectileType<CondemnationArrow>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * shootSpeed;

            Projectile.NewProjectile(tipPosition, shootVelocity, projectileType, damage, knockback, projectile.owner, 0f, 0f);
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            projectile.position = armPosition - projectile.Size * 0.5f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
        }

        public void ManipulatePlayerVariables()
		{
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }

        public override bool CanDamage() => false;
    }
}

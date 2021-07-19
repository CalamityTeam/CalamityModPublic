using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VehemenceHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];

        public const int ChargeTime = 90;
        public override string Texture => "CalamityMod/Items/Weapons/Magic/Vehemenc";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vehemence");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 114;
            projectile.friendly = false;
            projectile.magic = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
			projectile.timeLeft = ChargeTime + 1;
        }

        public override void AI()
        {
            UpdatePlayerVisuals();
            if (Time == ChargeTime)
                ShootBolt();
            else if (Time < ChargeTime)
                CreateChargeDust();

            Time++;

            if (Main.mouseLeftRelease && Time >= 5f && Time < ChargeTime)
                projectile.Kill();
        }

        private void UpdatePlayerVisuals()
        {
            projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            projectile.rotation = projectile.AngleTo(Main.MouseWorld);
            projectile.velocity = projectile.rotation.ToRotationVector2();

            // Place the projectile directly into the player's hand with an offset at all times.
            projectile.Center += projectile.rotation.ToRotationVector2() * 30f;

            projectile.direction = projectile.spriteDirection = (Math.Cos(projectile.rotation) > 0).ToDirectionInt();

            // The staff is a holdout projectile; change the player's variables to reflect this.
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = projectile.rotation * projectile.direction;

            projectile.rotation += MathHelper.PiOver4;
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.PiOver2;
        }

        private void ShootBolt()
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            Vector2 shootVelocity = (Main.MouseWorld - projectile.Center).SafeNormalize(Vector2.UnitX * projectile.direction) * heldItem.shootSpeed;
            Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<Vehemence>(), (int)(Owner.MagicDamage() * heldItem.damage), heldItem.knockBack, projectile.owner);
        }

        private void CreateChargeDust()
        {
            if (Main.dedServ)
                return;

            Vector2 spawnOffset = projectile.velocity * 94f;
            for (int i = 0; i < 18; i++)
            {
                Dust brimstoneMagic = Dust.NewDustPerfect(projectile.Center + spawnOffset + Main.rand.NextVector2CircularEdge(20f, 20f), (int)CalamityDusts.Brimstone);
                brimstoneMagic.velocity = (projectile.Center + spawnOffset - brimstoneMagic.position).SafeNormalize(Vector2.Zero) * 0.3f + Owner.velocity;
                brimstoneMagic.velocity.Y -= 2f;
                brimstoneMagic.scale = 1.2f;
                brimstoneMagic.noGravity = true;
            }
        }
    }
}

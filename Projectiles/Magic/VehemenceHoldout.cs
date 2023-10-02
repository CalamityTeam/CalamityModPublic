using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VehemenceHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Items.Weapons.Magic.Vehemence>();
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public ref float ChargeTime => ref Projectile.ai[1];

        public override string Texture => "CalamityMod/Items/Weapons/Magic/Vehemence";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 114;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 91;
        }

        public override void AI()
        {
            UpdatePlayerVisuals();
            if (Projectile.timeLeft > ChargeTime + 5)
                Projectile.timeLeft = (int)ChargeTime + 5;
            if (Time == ChargeTime)
                ShootBolt();
            else if (Time < ChargeTime)
                CreateChargeDust();

            Time++;

            // If the player aborts the charge by releasing LMB, it cancels.
            if (Main.mouseLeftRelease && Time >= 5f && Time < ChargeTime)
                Projectile.Kill();
        }

        private void UpdatePlayerVisuals()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
            Projectile.velocity = Projectile.rotation.ToRotationVector2();

            // Place the projectile directly into the player's hand with an offset at all times.
            Projectile.Center += Projectile.rotation.ToRotationVector2() * 30f;

            Projectile.direction = Projectile.spriteDirection = (Math.Cos(Projectile.rotation) > 0).ToDirectionInt();

            // The staff is a holdout projectile; change the player's variables to reflect this.
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = CalamityUtils.WrapAngle90Degrees(Projectile.rotation);

            Projectile.rotation += MathHelper.PiOver4;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.PiOver2;
        }

        private void ShootBolt()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            Vector2 shootVelocity = Projectile.velocity * heldItem.shootSpeed;
            int damage = heldItem is null ? 0 : Owner.GetWeaponDamage(heldItem);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<Vehemence>(), damage, heldItem.knockBack, Projectile.owner);
        }

        private void CreateChargeDust()
        {
            if (Main.dedServ)
                return;

            Vector2 spawnOffset = Projectile.velocity * 94f;
            for (int i = 0; i < 18; i++)
            {
                Dust brimstoneMagic = Dust.NewDustPerfect(Projectile.Center + spawnOffset + Main.rand.NextVector2CircularEdge(20f, 20f), (int)CalamityDusts.Brimstone);
                brimstoneMagic.velocity = (Projectile.Center + spawnOffset - brimstoneMagic.position).SafeNormalize(Vector2.Zero) * 0.3f + Owner.velocity;
                brimstoneMagic.velocity.Y -= 2f;
                brimstoneMagic.scale = 1.2f;
                brimstoneMagic.noGravity = true;
            }
        }
    }
}

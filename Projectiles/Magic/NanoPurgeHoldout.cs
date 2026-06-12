using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NanoPurgeHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<NanoPurge>();
        public override string Texture => "CalamityMod/Projectiles/Magic/NanoPurgeHoldout";
        public override float MaxOffsetLengthFromArm => 10f;
        public override float OffsetXUpwards => -15f;
        public override float OffsetXDownwards => 5f;
        public override float BaseOffsetY => -10f;
        public override float OffsetYUpwards => 5f;
        public override float OffsetYDownwards => 10f;

        private const int FramesPerFireRateIncrease = 36;
        private static int[] LaserOffsetByAnimationFrame = { 4, 3, 0, 3 };

        private ref float DeployedFrames => ref Projectile.ai[0];
        private ref float ChargeTowardsNextShot => ref Projectile.ai[1];

        public override void SetStaticDefaults() => Main.projFrames[Type] = 4;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public float postShotFade = 0;

        public override void KillHoldoutLogic()
        {
            base.KillHoldoutLogic();
            bool actuallyShoot = DeployedFrames >= (HeldItem?.useAnimation ?? NanoPurge.UseTime);
            bool manaOK = !actuallyShoot || Owner.CheckMana(Owner.HeldItem);
            if (!manaOK)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Projectile.damage = HeldItem is null ? 0 : Owner.GetWeaponDamage(HeldItem);

            // Get the original weapon's use time.
            int itemUseTime = HeldItem?.useAnimation ?? NanoPurge.UseTime;

            // Update time.
            DeployedFrames += 1f;

            // Choose fire rate multiplier (1x, 2x, 3x, 4x) based on current time spent firing.
            int fireRate = (int)MathHelper.Clamp(DeployedFrames / FramesPerFireRateIncrease, 1f, 4f);

            // Increment counter towards the item's use time by an amount equal to the current fire rate.
            ChargeTowardsNextShot += fireRate;

            // If enough charging progress is made, perform a shoot event if enough mana is available.
            if (ChargeTowardsNextShot >= itemUseTime)
            {
                ChargeTowardsNextShot -= itemUseTime;

                // Update the animation.
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];

                bool manaCostPaid = Owner.CheckMana(Owner.HeldItem, -1, true, false);
                if (manaCostPaid)
                {
                    postShotFade = 1;
                    SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);

                    if (Main.myPlayer == Projectile.owner)
                    {
                        int projID = ModContent.ProjectileType<NanoPurgeLaser>();
                        float shootSpeed = HeldItem.shootSpeed;
                        float inaccuracyRatio = 0.045f;
                        Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                        Vector2 perp = shootDirection.RotatedBy(MathHelper.PiOver2);

                        // Fire a pair of lasers, one with a negative offset, one with a positive offset.
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 spread = Main.rand.NextVector2CircularEdge(shootSpeed, shootSpeed);
                            Vector2 shootVelocity = shootDirection * shootSpeed + inaccuracyRatio * spread;
                            Vector2 splitBarrelPos = GunTipPosition + i * LaserOffsetByAnimationFrame[Projectile.frame] * perp;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), splitBarrelPos, shootVelocity, projID, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            SpawnFiringDust(splitBarrelPos, shootVelocity);
                        }
                    }
                }
            }

            ExtraBackArmRotation = Utils.Remap(Vector2.Dot(-Vector2.UnitY, Projectile.velocity.SafeNormalize(-Vector2.UnitY)), 0f, 1f, MathHelper.PiOver4, 0f);
            postShotFade *= 0.86f;
        }

        private void SpawnFiringDust(Vector2 GunTipPosition, Vector2 laserVelocity)
        {
            int dustID = ModContent.DustType<LightDust>();
            int dustRadius = 5;
            int dustDiameter = 2 * dustRadius;
            Vector2 dustCorner = GunTipPosition - Vector2.One * dustRadius;
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustVel = laserVelocity + Main.rand.NextVector2Circular(7f, 7f);
                Dust d = Dust.NewDustDirect(dustCorner, dustDiameter, dustDiameter, dustID, dustVel.X, dustVel.Y);
                d.velocity *= 0.125f;
                d.noGravity = true;
                d.scale = 0.9f;
                d.color = Color.Lime;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            FrontArmStretch = Player.CompositeArmStretchAmount.Quarter;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 vel = Projectile.velocity * 10;
            Vector2 drawPosition = (Projectile.Center - Main.screenPosition) - vel * postShotFade;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) + MathHelper.PiOver2;
            Vector2 rotationPoint = frame.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}

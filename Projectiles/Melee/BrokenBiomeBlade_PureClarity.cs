using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PureClarity : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override int AssignedItemID => ModContent.ItemType<BrokenBiomeBlade>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/BrokenBiomeBlade";
        public override float HitboxOutset => 50f;
        public override Vector2 HitboxSize => new Vector2(48, 48);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override Vector2 SpriteOrigin => new(-5, 40);

        public ref float SwingDir => ref Projectile.ai[1];
        public Vector2 mousePos;
        public Vector2 aimPos;
        public bool doSwing = true;
        public bool postSwing = false;
        public int useAnimation;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1.25f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void WhenSpawned()
        {
            SwingDir = 1f;
            mousePos = Owner.Calamity().mouseWorld;
            aimPos = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnimation = Owner.itemAnimationMax;

            Owner.direction = (mousePos.X < Owner.Center.X) ? -1 : 1;
            FlipAsSword = Owner.direction == -1 ? true : false;
        }

        public override void UseStyle()
        {
            if (Owner.HeldItem.ModItem is not BrokenBiomeBlade || (Owner.HeldItem.ModItem as BrokenBiomeBlade).mainAttunement != AttunementSystem.FindOrNull(AttunementID.Default))
            {
                Projectile.Kill();
                return;
            }

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimPos;
            else
                mousePos = Owner.Calamity().mouseWorld;

            if (!doSwing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                mousePos = Owner.Calamity().mouseWorld;
                aimPos = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                Owner.direction = (mousePos.X < Owner.Center.X) ? -1 : 1;
                FlipAsSword = Owner.direction == -1 ? true : false;
                doSwing = true;
            }
            else
            {
                if (!CanHit && !postSwing)
                    Owner.direction = (mousePos.X < Owner.Center.X) ? -1 : 1;
                else
                    Owner.direction = ((Owner.Center - aimPos).X < Owner.Center.X) ? -1 : 1;
                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(65f), 0.1f);

                if (AnimationProgress < (useAnimation / 3))
                {
                    // Swing wind-up. Should not deal damage.
                    aimPos = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        doSwing = false;
                        SwingDir = -SwingDir;
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * SwingDir * Owner.direction), 0.2f);
                }
                else
                {
                    float swingTime = AnimationProgress - (useAnimation / 3);
                    float swingTimeMax = useAnimation - (useAnimation / 3);

                    if (swingTime > (int)(swingTimeMax * 0.2f) && swingTime < (int)(swingTimeMax * 0.85f))
                    {
                        CanHit = true;

                        // Particle effects on swing.
                        Vector2 particleVel = new Vector2(0, 10 * -SwingDir * Owner.direction).RotatedBy(FinalRotation - MathHelper.PiOver4);
                        Vector2 particlePos = Owner.Center + new Vector2(Main.rand.Next(0, 80), 0).RotatedBy(FinalRotation - MathHelper.PiOver4);
                        Color particleColor = (Owner.HeldItem.ModItem as BrokenBiomeBlade).mainAttunement.tooltipColor;
                        if (Main.rand.NextBool())
                        {
                            GenericBloom bloom = new(particlePos, particleVel, particleColor, 0.08f, 20);
                            GeneralParticleHandler.SpawnParticle(bloom);
                        }
                        else
                        {
                            GenericSparkle sparkle = new(particlePos, particleVel, particleColor, particleColor, 0.55f, 20);
                            GeneralParticleHandler.SpawnParticle(sparkle);
                        }
                    }
                    else
                        CanHit = false;

                    // Fire projectiles.
                    if (swingTime == (int)(swingTimeMax * 0.4f))
                    {
                        SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.65f }, Projectile.Center);
                        Vector2 projVel = (Owner.Calamity().mouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX) * 14.5f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, projVel, ModContent.ProjectileType<PurityProjection>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    }

                    RotationOffset = MathHelper.Lerp(RotationOffset,
                        MathHelper.ToRadians(MathHelper.Lerp(150f * SwingDir * Owner.direction, 120f * -SwingDir * Owner.direction, CalamityUtils.ExpInOutEasing(swingTime / swingTimeMax, 1))),
                        0.2f);

                    if (swingTime >= swingTimeMax)
                        doSwing = false;
                    if (swingTime < (int)(swingTimeMax * 0.7f))
                        postSwing = true;
                }
            }

            // Make the player's arms rotate.
            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }

        public override void ResetStyle()
        {
        }
    }
}

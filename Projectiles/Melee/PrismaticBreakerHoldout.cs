using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Tools;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismaticBreakerHoldout : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public override int AssociatedItemID => ModContent.ItemType<PrismaticBreaker>();
        public override float MaxOffsetLengthFromArm => 40f;

        public ref float Timer => ref Projectile.ai[0];
        public const float LaserChargeTime = 300f;
        public const float LaserAimLag = 0.94f; // Last Prism-esque aim lag. Higher = Slower. Last Prism is 0.92f.
        public const float LaserDamageMult = 2f;
        public const float LaserLifetime = 360f;

        public float StarTimer = 0f;
        public float StarFrequency = 47f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = MeleeRangedHybridDamageClass.Instance;
        }

        public override void HoldoutAI()
        {
            if (Owner.CantUseHoldout())
            {
                if (!CalamityUtils.AnyProjectiles(ModContent.ProjectileType<PrismaticMagicCircle>()))
                    Projectile.Kill();
            }
            if (Timer > LaserChargeTime + LaserLifetime)
                Projectile.Kill();

            Timer++;
            if (Timer <= LaserChargeTime)
            {
                StarTimer++;
                if (StarTimer >= StarFrequency && Main.myPlayer == Projectile.owner)
                {
                    StarTimer = 0f;
                    SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.6f }, Projectile.Center);
                    for (int i = 0; i < 3; i++)
                    {
                        float clampedChargeTime = MathHelper.Clamp(Timer / LaserChargeTime, 0f, 1f);
                        float starOffset = MathHelper.Lerp(MathHelper.Pi / 6f, 0f, clampedChargeTime);
                        Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedByRandom(starOffset) * 17f;

                        Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, velocity, ModContent.ProjectileType<PrismaticWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Main.rand.Next(12));
                        star.scale = MathHelper.Lerp(0.75f, 1f, clampedChargeTime);
                    }
                    StarFrequency -= 4f;
                }
            }

            if (Timer == LaserChargeTime - 100f)
                SoundEngine.PlaySound(CrystylCrusher.ChargeSound, Projectile.Center, _ => new ProjectileAudioTracker(Projectile).IsActiveAndInGame());
            if (Timer == LaserChargeTime && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.velocity, ModContent.ProjectileType<PrismaticMagicCircle>(), (int)(Projectile.damage * LaserDamageMult), Projectile.knockBack, Projectile.owner);
            }
        }

        public override void ManageHoldout()
        {
            Vector2 storedVelocity = Projectile.velocity;
            base.ManageHoldout();

            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<PrismaticMagicCircle>()] > 0)
            {
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 aimVector = (Main.MouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter, true)).SafeNormalize(Vector2.UnitY);
                aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(storedVelocity), LaserAimLag));

                if (aimVector != storedVelocity)
                    Projectile.netUpdate = true;
                Projectile.velocity = aimVector;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        // We need to rotate the drawing by 45 degrees since the sword sprite points up-right
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (MathHelper.PiOver4 * Projectile.spriteDirection) + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.PiOver2 * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}

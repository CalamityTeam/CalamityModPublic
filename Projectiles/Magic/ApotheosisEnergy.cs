using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ApotheosisEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 210;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * Projectile.Opacity);

            // Fade-effects.
            if (Projectile.timeLeft < 40f)
                Projectile.Opacity = Utils.GetLerpValue(0f, 40f, Projectile.timeLeft, true);
            else
                Projectile.alpha = Utils.Clamp(Projectile.alpha - 30, 0, 255);

            NPC potentialTarget = Projectile.Center.ClosestNPCAt(5900f);
            if (potentialTarget != null)
            {
                float squaredTargetDistance = Projectile.DistanceSQ(potentialTarget.Center);
                if (squaredTargetDistance > 120f * 120f && squaredTargetDistance < 1000f * 1000f)
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(potentialTarget.Center), 0.1f).ToRotationVector2() * Projectile.velocity.Length();
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            Color baseColor = Color.Cyan;
            if (completionRatio > 0.66f)
                baseColor = Color.Lerp(baseColor, Color.Fuchsia, (completionRatio - 0.66f) / 0.33f);
            else
            {
                float whiteFade = (float)Math.Sin(Utils.GetLerpValue(0.0f, 0.2f, completionRatio, true) * MathHelper.Pi + Main.GlobalTimeWrappedHourly * 3f) * 0.45f;
                baseColor = Color.Lerp(baseColor, Color.White, whiteFade);
            }

            Color colorToUse = baseColor;
            if (completionRatio > 0.5f)
                colorToUse = Color.Lerp(baseColor, Color.Transparent, 1f - completionRatio / 0.5f);
            return colorToUse * Projectile.Opacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float width;
            float maxWidthOutwardness = 6f;
            if (completionRatio < 0.2f)
                width = (float)Math.Sin(completionRatio / 0.2f * MathHelper.PiOver2) * maxWidthOutwardness + 0.1f;
            else
                width = MathHelper.Lerp(maxWidthOutwardness, 0f, Utils.GetLerpValue(0.2f, 1f, completionRatio, true));
            return width * Projectile.Opacity;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Collision.CheckAABBvAABBCollision(Projectile.oldPos[i], projHitbox.Size(), targetHitbox.TopLeft(), projHitbox.Size()))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f), 85);
            return false;
        }
    }
}

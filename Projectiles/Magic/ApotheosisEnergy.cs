using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ApotheosisEnergy : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apotheosis Energy");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 210;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Cyan.ToVector3() * projectile.Opacity);

            // Fade-effects.
            if (projectile.timeLeft < 40f)
                projectile.Opacity = Utils.InverseLerp(0f, 40f, projectile.timeLeft, true);
            else
                projectile.alpha = Utils.Clamp(projectile.alpha - 30, 0, 255);

            NPC potentialTarget = projectile.Center.ClosestNPCAt(5900f);
            if (potentialTarget != null)
            {
                float squaredTargetDistance = projectile.DistanceSQ(potentialTarget.Center);
                if (squaredTargetDistance > 120f * 120f && squaredTargetDistance < 1000f * 1000f)
                    projectile.velocity = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(potentialTarget.Center), 0.1f).ToRotationVector2() * projectile.velocity.Length();
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            Color baseColor = Color.Cyan;
            if (completionRatio > 0.66f)
                baseColor = Color.Lerp(baseColor, Color.Fuchsia, (completionRatio - 0.66f) / 0.33f);
            else
            {
                float whiteFade = (float)Math.Sin(Utils.InverseLerp(0.0f, 0.2f, completionRatio, true) * MathHelper.Pi + Main.GlobalTime * 3f) * 0.45f;
                baseColor = Color.Lerp(baseColor, Color.White, whiteFade);
            }

            Color colorToUse = baseColor;
            if (completionRatio > 0.5f)
                colorToUse = Color.Lerp(baseColor, Color.Transparent, 1f - completionRatio / 0.5f);
            return colorToUse * projectile.Opacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float width;
            float maxWidthOutwardness = 6f;
            if (completionRatio < 0.2f)
                width = (float)Math.Sin(completionRatio / 0.2f * MathHelper.PiOver2) * maxWidthOutwardness + 0.1f;
            else
                width = MathHelper.Lerp(maxWidthOutwardness, 0f, Utils.InverseLerp(0.2f, 1f, completionRatio, true));
            return width * projectile.Opacity;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                if (Collision.CheckAABBvAABBCollision(projectile.oldPos[i], projHitbox.Size(), targetHitbox.TopLeft(), projHitbox.Size()))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction);

            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 85);
            return false;
        }
    }
}

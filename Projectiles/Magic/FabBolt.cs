using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class FabBolt : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool FadingOut
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value.ToInt();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 23;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.extraUpdates = 2;
            projectile.penetrate = 12;
            projectile.timeLeft = 90 * projectile.extraUpdates;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (FadingOut)
            {
                projectile.Opacity = MathHelper.Lerp(projectile.Opacity, 0f, 0.27f);
                if (projectile.Opacity <= 0.05f)
                    projectile.Kill();
            }
            else
                projectile.velocity *= 1.004f;
            projectile.rotation = projectile.velocity.ToRotation();

            // Emit light.
            Lighting.AddLight(projectile.Center, Vector3.One * projectile.Opacity * 0.45f);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.25f, 0.5f, (float)Math.Cos(-Main.GlobalTime * 3f) * 0.5f + 0.5f);
            fadeToEnd *= 1f - Utils.InverseLerp(0.35f, 0f, completionRatio, true);
            Color endColor = Color.Lerp(Color.Cyan, Color.HotPink, projectile.identity % 2);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * projectile.Opacity * 0.7f;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.InverseLerp(0f, 0.2f, completionRatio, true), 2D);
            return MathHelper.Lerp(0f, 22f, expansionCompletion) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction, GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/FabstaffStreak"));
            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 80, projectile.oldRot);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!FadingOut && projectile.penetrate < 2)
            {
                FadingOut = true;
                projectile.velocity *= 0.1f;
                projectile.extraUpdates = 0;
                projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!FadingOut)
            {
                FadingOut = true;
                projectile.velocity *= 0.1f;
                projectile.extraUpdates = 0;
                projectile.netUpdate = true;
            }
            return false;
        }

        public override bool CanDamage()
        {
            if (FadingOut)
                return false;
            return base.CanDamage();
        }
    }
}

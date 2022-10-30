using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CalamityMod.Projectiles.Melee
{
    public class TerratomereBeam : ModProjectile
    {
        public Vector2[] ControlPoints;
        
        public PrimitiveTrail SlashDrawer = null;

        public bool Flipped => Projectile.ai[0] == 1f;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Slash Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 144;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.Opacity = Utils.GetLerpValue(0f, 36f, Projectile.timeLeft, true);
            Projectile.velocity *= 0.955f;
            Projectile.scale *= 1.01f;
        }

        public float SlashWidthFunction(float completionRatio) => Projectile.scale * 22f;

        public Color SlashColorFunction(float completionRatio) => Color.Lime * Utils.GetLerpValue(0.04f, 0.27f, completionRatio, true) * Projectile.Opacity;
        
        public override bool PreDraw(ref Color lightColor)
        {
            // Initialize the primitive drawer.
            SlashDrawer ??= new(SlashWidthFunction, SlashColorFunction, null, GameShaders.Misc["CalamityMod:ExobladeSlash"]);

            // Draw the slash effect.
            Main.spriteBatch.EnterShaderRegion();
            
            TerratomereHoldoutProj.PrepareSlashShader(Flipped);

            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < ControlPoints.Length; i++)
                points.Add(ControlPoints[i] + ControlPoints[i].SafeNormalize(Vector2.Zero) * (Projectile.scale - 1f) * 40f);

            for (int i = 0; i < 3; i++)
                SlashDrawer.Draw(points, Projectile.Center - Main.screenPosition, 65);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());
    }
}

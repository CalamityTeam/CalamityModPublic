using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DyingSun : ModProjectile
    {
        public PrimitiveTrail FireDrawer;
        public ref float Time => ref projectile.ai[0];
        public ref float Radius => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults() => DisplayName.SetDefault("Dying Sun");

        public override void SetDefaults()
        {
            projectile.width = 164;
            projectile.height = 164;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 100;
            projectile.MaxUpdates = 2;
            projectile.scale = 1f;
        }

        public override void AI()
        {
            projectile.scale += 0.08f;
            Radius = projectile.scale * 42f;
            projectile.Opacity = Utils.InverseLerp(8f, 42f, projectile.timeLeft, true);

            Time++;
        }

        public float SunWidthFunction(float completionRatio) => Radius * projectile.scale * (float)Math.Sin(MathHelper.Pi * completionRatio);

        public Color SunColorFunction(float completionRatio)
        {
            Color sunColor = Main.dayTime ? Color.Yellow : Color.Cyan;
            return Color.Lerp(sunColor, Color.White, (float)Math.Sin(MathHelper.Pi * completionRatio) * 0.5f + 0.3f) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (FireDrawer is null)
                FireDrawer = new PrimitiveTrail(SunWidthFunction, SunColorFunction, null, GameShaders.Misc["CalamityMod:Flame"]);

            GameShaders.Misc["CalamityMod:Flame"].UseSaturation(0.45f);
            GameShaders.Misc["CalamityMod:Flame"].UseImage("Images/Misc/Perlin");

            List<float> rotationPoints = new List<float>();
            List<Vector2> drawPoints = new List<Vector2>();

            for (float offsetAngle = -MathHelper.PiOver2; offsetAngle <= MathHelper.PiOver2; offsetAngle += MathHelper.Pi / 80f)
            {
                rotationPoints.Clear();
                drawPoints.Clear();

                float adjustedAngle = offsetAngle + MathHelper.Pi * -0.2f;
                Vector2 offsetDirection = adjustedAngle.ToRotationVector2();
                for (int i = 0; i < 16; i++)
                {
                    rotationPoints.Add(adjustedAngle);
                    drawPoints.Add(Vector2.Lerp(projectile.Center - offsetDirection * Radius / 2f, projectile.Center + offsetDirection * Radius / 2f, i / 16f));
                }

                FireDrawer.Draw(drawPoints, -Main.screenPosition, 24);
            }
            return false;
        }
    }
}

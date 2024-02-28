using System;
using System.Collections.Generic;
using CalamityMod.Graphics.Primitives;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DyingSun : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";        
        public ref float Time => ref Projectile.ai[0];
        public ref float Radius => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.MaxUpdates = 2;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Projectile.scale += 0.08f;
            Radius = Projectile.scale * 42f;
            Projectile.Opacity = Utils.GetLerpValue(8f, 42f, Projectile.timeLeft, true);

            Time++;
        }

        public float SunWidthFunction(float completionRatio) => Radius * Projectile.scale * (float)Math.Sin(MathHelper.Pi * completionRatio);

        public Color SunColorFunction(float completionRatio)
        {
            Color sunColor = Main.zenithWorld ? (new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB) * 0.8f) : (Main.dayTime ? Color.Yellow : Color.Cyan);
            return Color.Lerp(sunColor, Color.White, (float)Math.Sin(MathHelper.Pi * completionRatio) * 0.5f + 0.3f) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:Flame"].UseSaturation(0.45f);
            GameShaders.Misc["CalamityMod:Flame"].UseImage1("Images/Misc/Perlin");

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
                    drawPoints.Add(Vector2.Lerp(Projectile.Center - offsetDirection * Radius / 2f, Projectile.Center + offsetDirection * Radius / 2f, i / 16f));
                }
                PrimitiveSet.Prepare(drawPoints, new(SunWidthFunction, SunColorFunction, shader: GameShaders.Misc["CalamityMod:Flame"]), 24);
            }
            return false;
        }
    }
}

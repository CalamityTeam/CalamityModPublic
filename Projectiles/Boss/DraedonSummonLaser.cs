using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DraedonSummonLaser : ModProjectile
    {
        internal static readonly Color[] ExoPalette = new Color[]
        {
            new Color(250, 255, 112),
            new Color(211, 235, 108),
            new Color(166, 240, 105),
            new Color(105, 240, 220),
            new Color(64, 130, 145),
            new Color(145, 96, 145),
            new Color(242, 112, 73),
            new Color(199, 62, 62),
        };

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public PrimitiveTrail RayDrawer = null;
        private const int Lifetime = CalamityWorld.DraedonSummonCountdownMax - 150;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Giant Fuck-off Deathray of Doom");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 24;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.timeLeft = Lifetime;
        }

        public override bool CanDamage() => projectile.timeLeft < Lifetime - 30;

        public override void AI()
		{
            if (projectile.localAI[0] == 0f)
			{
                for (int i = 0; i < 36; i++)
				{
                    Dust exoDust = Dust.NewDustPerfect(projectile.BottomRight, 267);
                    exoDust.color = CalamityUtils.MulticolorLerp(i / 36f, ExoPalette);
                    exoDust.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * new Vector2(3f, 1.45f) - Vector2.UnitY * 2f;
                    exoDust.scale = 2.8f;
                    exoDust.fadeIn = Main.rand.NextFloat(0.8f, 1.85f);
                    exoDust.noGravity = true;
				}

                for (int i = 0; i < 10; i++)
                {
                    Dust exoDust = Dust.NewDustPerfect(projectile.BottomRight, 267);
                    exoDust.color = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), ExoPalette);
                    exoDust.velocity = Main.rand.NextVector2Circular(2f, 2f);
                    exoDust.scale = 4f;
                    exoDust.fadeIn = 2f;
                    exoDust.noGravity = true;
                }
                projectile.localAI[0] = 1f;
			}

            Lighting.AddLight(projectile.Center, Color.White.ToVector3());
            projectile.scale = Utils.InverseLerp(-1f, 15f, projectile.timeLeft, true) * Utils.InverseLerp(Lifetime + 1f, Lifetime - 15f, projectile.timeLeft, true);
        }

        private float PrimitiveWidthFunction(float completionRatio)
        {
            return Utils.InverseLerp(1f, 0.96f, completionRatio, true) * Utils.InverseLerp(0f, 0.016f, completionRatio, true) * projectile.scale * 20f;
        }

        private Color PrimitiveColorFunction(float completionRatio)
		{
            return CalamityUtils.MulticolorLerp((Main.GlobalTime * 0.67f - completionRatio * 3f) % 1f, ExoPalette) * 1.2f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (RayDrawer is null)
                RayDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:Flame"]);

            GameShaders.Misc["CalamityMod:Flame"].UseImage("Images/Misc/Perlin");

            Vector2[] basePoints = new Vector2[8];
            for (int i = 0; i < basePoints.Length; i++)
                basePoints[i] = projectile.Center - Vector2.UnitY * i / basePoints.Length * 3800f;

            Vector2 overallOffset = projectile.Size * 0.5f - Main.screenPosition;
            RayDrawer.Draw(basePoints, overallOffset, 92);
            return false;
        }

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }
	}
}

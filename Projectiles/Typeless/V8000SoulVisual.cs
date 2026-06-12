using System;
using System.Linq;
using CalamityMod.DataStructures;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class V8000SoulVisual : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
            Projectile.Opacity = 1f;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 10 == 0)
                Projectile.ai[1] = Main.rand.NextFloat(-0.25f, 0.25f);

            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);

            Projectile.velocity *= 0.98f;

            if (Main.rand.NextBool(5))
            {
                Vector2 smokeVelocity = Main.rand.NextVector2Circular(1f, 1f) * 0.5f;
                HeavySmokeParticle ghastlySmoke = new(Projectile.Center, smokeVelocity, Color.DodgerBlue, Main.rand.Next(20, 35), Main.rand.NextFloat(0.15f, 0.25f), Projectile.Opacity * 0.8f, 0.0175f, true);
                GeneralParticleHandler.SpawnParticle(ghastlySmoke);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float WidthFunction(float completion, Vector2 _)
        {
            float maxBodyWidth = Projectile.scale * 14f;

            if (completion < 0.2f)
                return MathF.Sin(completion / 0.2f * MathHelper.PiOver2) * maxBodyWidth;

            return MathHelper.Lerp(maxBodyWidth, 0f, (completion - 0.2f) / 0.8f);
        }

        public Color ColorFunction(float completion, Vector2 _)
        {
            Color baseColor = Color.DodgerBlue;
            return baseColor * Projectile.Opacity * (1f - completion);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            var shader = GameShaders.Misc["CalamityMod:ImpFlameTrail"];

            shader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_, _) => Projectile.Size * 0.5f, true, true, shader), Projectile.oldPos.Length * 2);

            Vector2[] corePos = Projectile.oldPos.Take(4).ToArray();
            shader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(corePos, new((c, _) => WidthFunction(c, _) * 0.6f, (c, _) => Color.White * Projectile.Opacity * (1f - c), (_, _) => Projectile.Size * 0.5f, true, true, shader), corePos.Length * 2);
        }
    }
}

using CalamityMod.Particles;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Projectiles.Melee
{
	public class RendingNeedle : ModProjectile 
    {

        internal PrimitiveTrail TrailDrawer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rending Needle");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        const float MaxTime = 30;
        public float Timer => MaxTime - projectile.timeLeft;

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = (int)MaxTime;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 44f * projectile.scale;
            Vector2 start = -Utils.SafeNormalize(projectile.velocity, Vector2.Zero) * 16f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + start, projectile.Center + start + Utils.SafeNormalize(projectile.velocity, Vector2.Zero) * bladeLenght, 24, ref collisionPoint);
        }

        public override void AI()
        {
            projectile.scale = 2.4f;
            projectile.Opacity = 0.6f;
            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            projectile.velocity *= (1 - (float)Math.Pow(Timer / MaxTime, 3) * 0.3f);

            if (Main.rand.Next(2) == 0)
            {
                Particle smoke = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * projectile.scale * 0.6f, 0.28f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Main.hslToRgb(Main.rand.NextFloat(), 1, 0.7f), 20, Main.rand.NextFloat(0.4f, 0.7f) * projectile.scale * 0.6f, 0.8f, 0, true, 0.05f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }

            if (projectile.velocity.Length() < 1.0f)
                projectile.Kill();
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-Main.GlobalTime * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.InverseLerp(1f, 0.64f, completionRatio, true) * projectile.Opacity;

            Color endColor = Color.Lerp(Color.Cyan, Color.Crimson, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTime * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = (float)Math.Pow(1 - completionRatio, 2);
            return MathHelper.Lerp(0f, 14f * projectile.scale, expansionCompletion);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 35)
                return false;

            Texture2D texture = GetTexture("CalamityMod/Projectiles/Melee/RendingNeedle");

            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));
            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Utils.SafeNormalize(projectile.velocity, Vector2.Zero) * 30.5f - Main.screenPosition, 30);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), projectile.rotation, texture.Size() / 2f, projectile.scale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D starTexture = GetTexture("CalamityMod/Particles/Sparkle");
            Texture2D bloomTexture = GetTexture("CalamityMod/Particles/BloomCircle");
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)starTexture.Height / (float)bloomTexture.Height;

            Color color = Main.hslToRgb((Main.GlobalTime * 0.6f) % 1, 1, 0.85f);
            float rotation = Main.GlobalTime * 8f;

            Vector2 sparkCenter = projectile.Center - Utils.SafeNormalize(projectile.velocity, Vector2.Zero) * 30.5f - Main.screenPosition;

            spriteBatch.Draw(bloomTexture, sparkCenter, null, color* 0.5f, 0, bloomTexture.Size() / 2f, 4 * properBloomSize, SpriteEffects.None, 0);
            spriteBatch.Draw(starTexture, sparkCenter, null, color * 0.5f, rotation + MathHelper.PiOver4, starTexture.Size() / 2f, 2 * 0.75f, SpriteEffects.None, 0);
            spriteBatch.Draw(starTexture, sparkCenter, null, Color.White, rotation, starTexture.Size() / 2f, 2, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.DD2_WitherBeastDeath, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1.2f, 2.3f);
                Particle energyLeak = new SquishyLightParticle(projectile.Center + Utils.SafeNormalize(projectile.velocity, Vector2.Zero) * 40f, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 particleOrigin = target.Hitbox.Size().Length() < 140 ? target.Center : projectile.Center + projectile.rotation.ToRotationVector2() * 60f;
            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(particleOrigin, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }
    }
}
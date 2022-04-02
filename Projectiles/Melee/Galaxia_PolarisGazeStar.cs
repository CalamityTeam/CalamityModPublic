using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class PolarisGazeStar : ModProjectile
    {
        public override string Texture => "CalamityMod/Particles/Sparkle";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (PolarisGaze.maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];

        public float Timer => MaxTime - projectile.timeLeft;

        public const float MaxTime = 120;

        public Particle PolarStar; //Using a particle ontop of it since the smoke particles would otherwise go over it


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Northern Star");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 45;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= 0.95f;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 84 * projectile.scale;
            float bladeWidth = 76 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - direction * bladeLenght / 2, projectile.Center + direction * bladeLenght / 2, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized)
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;

                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                projectile.timeLeft = (int)MaxTime;
                projectile.velocity = direction * 16f;

                projectile.scale = 1f + ShredRatio; //SWAGGER
                projectile.netUpdate = true;

            }

            projectile.velocity *= 0.96f;
            projectile.position += projectile.velocity;


            if (PolarStar == null)
            {
                PolarStar = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Color.CornflowerBlue, projectile.scale * 2f, 2, 0.1f, 5f, true);
                GeneralParticleHandler.SpawnParticle(PolarStar);
            }
            else
            {
                PolarStar.Time = 0;
                PolarStar.Position = projectile.Center;
                PolarStar.Scale = projectile.scale * 2f;
            }


            Vector2 smokeSpeed = Main.rand.NextVector2Circular(10f, 10f);
            Particle smoke = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.Next(3) == 0)
            {
                Particle smokeGlow = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Main.hslToRgb(0.55f, 1, 0.5f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0, true, 0.01f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            var tex = GetTexture("CalamityMod/Particles/Sparkle");
            float opacityFade = projectile.timeLeft > 15 ? 1 : projectile.timeLeft / 15f;

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.5f * opacityFade, Main.GlobalTime * 10f + MathHelper.PiOver4, tex.Size() / 2f, projectile.scale * 1.5f, 0f, 0f);

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, Main.GlobalTime * 10f, tex.Size() / 2f, projectile.scale * 2f, 0f, 0f);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item60, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.CornflowerBlue : Color.MediumSlateBlue, 0.4f + Main.rand.NextFloat(0f, 3.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);

                Vector2 smokeSpeed = Main.rand.NextVector2Circular(20f, 20f);
                Particle smoke = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Color.Lerp(Color.DarkRed, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(1.5f, 2.2f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Main.hslToRgb(0.55f, 1, 0.5f), 20, Main.rand.NextFloat(1.4f, 1.5f), 0.8f, 0, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

    }
}
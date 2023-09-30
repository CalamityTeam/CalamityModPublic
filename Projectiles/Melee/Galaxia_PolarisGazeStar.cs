using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class PolarisGazeStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Particles/Sparkle";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref Projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (PolarisGaze.maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[Projectile.owner];

        public float Timer => MaxTime - Projectile.timeLeft;

        public const float MaxTime = 120;

        public Particle PolarStar; //Using a particle ontop of it since the smoke particles would otherwise go over it


        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 45;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.95f;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 84 * Projectile.scale;
            float bladeWidth = 76 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - direction * bladeLength / 2, Projectile.Center + direction * bladeLength / 2, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item90, Projectile.Center);
                initialized = true;

                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();

                Projectile.timeLeft = (int)MaxTime;
                Projectile.velocity = direction * 16f;

                Projectile.scale = 1f + ShredRatio; //SWAGGER
                Projectile.netUpdate = true;

            }

            Projectile.velocity *= 0.96f;
            Projectile.position += Projectile.velocity;


            if (PolarStar == null)
            {
                PolarStar = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Color.CornflowerBlue, Projectile.scale * 2f, 2, 0.1f, 5f, true);
                GeneralParticleHandler.SpawnParticle(PolarStar);
            }
            else
            {
                PolarStar.Time = 0;
                PolarStar.Position = Projectile.Center;
                PolarStar.Scale = Projectile.scale * 2f;
            }


            Vector2 smokeSpeed = Main.rand.NextVector2Circular(10f, 10f);
            Particle smoke = new HeavySmokeParticle(Projectile.Center, smokeSpeed + Projectile.velocity / 2, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.Next(3) == 0)
            {
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, smokeSpeed + Projectile.velocity / 2, Main.hslToRgb(0.55f, 1, 0.5f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0, true, 0.01f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            var tex = Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
            float opacityFade = Projectile.timeLeft > 15 ? 1 : Projectile.timeLeft / 15f;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.5f * opacityFade, Main.GlobalTimeWrappedHourly * 10f + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale * 1.5f, 0f, 0);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, Main.GlobalTimeWrappedHourly * 10f, tex.Size() / 2f, Projectile.scale * 2f, 0f, 0);

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * Projectile.scale, Color.White, Main.rand.NextBool() ? Color.CornflowerBlue : Color.MediumSlateBlue, 0.4f + Main.rand.NextFloat(0f, 3.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);

                Vector2 smokeSpeed = Main.rand.NextVector2Circular(20f, 20f);
                Particle smoke = new HeavySmokeParticle(Projectile.Center, smokeSpeed + Projectile.velocity / 2, Color.Lerp(Color.DarkRed, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(1.5f, 2.2f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, smokeSpeed + Projectile.velocity / 2, Main.hslToRgb(0.55f, 1, 0.5f), 20, Main.rand.NextFloat(1.4f, 1.5f), 0.8f, 0, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

    }
}

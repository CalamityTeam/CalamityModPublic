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
    public class TrueAridGrandeurShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeurExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref Projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (TrueAridGrandeur.maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[Projectile.owner];

        public const float pogoStrenght = 16f; //How much the player gets pogoed up

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
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
                Projectile.timeLeft = (int)(30f + ShredRatio * 30f);
                initialized = true;

                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();

                Projectile.velocity = direction * 6f;

                Projectile.scale = 1f + ShredRatio; //SWAGGER
                Projectile.netUpdate = true;

            }

            Projectile.position += Projectile.velocity;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++) //Draw extra copies
            {
                var tex = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_AridGrandeurExtra").Value;

                float drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5 + i * MathHelper.PiOver2);
                float drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);

                Vector2 drawOrigin = new Vector2(0f, tex.Height);


                Vector2 drawOffsetStraight = Projectile.Center + direction * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                float opacityFade = Projectile.timeLeft > 15 ? 1 : Projectile.timeLeft / 15f;

                Main.EntitySpriteDraw(tex, drawOffsetStraight + drawDisplacementAngle, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            }

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            for (int i = 0; i < 4; i++) //Particel
            {
                float drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5 + i * MathHelper.PiOver2);
                float drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);


                Vector2 drawOffsetStraight = Projectile.Center + direction * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7) * 10; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                for (int j = 0; j < 4; j++)
                {
                    Particle Sparkle = new GenericSparkle(drawOffsetStraight + (drawRotation - MathHelper.PiOver4).ToRotationVector2() * (60 + j * 50f) + drawDisplacementAngle, direction * Projectile.velocity.Length() * Main.rand.NextFloat(0.9f, 1.1f), Color.Lerp(Color.Cyan, Color.Orange, (j + 1) / 4f), Color.OrangeRed, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

            }

        }

    }
}

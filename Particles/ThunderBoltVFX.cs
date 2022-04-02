using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityMod.Particles
{
    public class ThunderBoltVFX : Particle //Also check out Split mod!
    {
        public override string Texture => "CalamityMod/Particles/ThunderBolt";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        public Vector2 Squish;
        public float ShakePower;
        public float Opacity;
        public Projectile AttachedProjectile;
        public float  DisplacementFromProjectile;

        public ThunderBoltVFX(Vector2 position, Color color, float rotation, float scale, Vector2 squish, float opacity = 1f, float shakePower = 20f, Projectile projectileToFollow = null, float displacementFromProjectile = 0f)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Color = color;
            Scale = scale;
            Squish = squish;
            Rotation = rotation;
            Opacity = opacity;
            ShakePower = shakePower;
            if (projectileToFollow != null)
                AttachedProjectile = projectileToFollow;
            DisplacementFromProjectile = displacementFromProjectile;
            Lifetime = 30;
        }

        public override void Update()
        {
            Lighting.AddLight(Position, Color.ToVector3() * 3f);
            float fadeFactor = 1f - 0.05f * MathHelper.Clamp((Time - 10) / 10f, 0f, 1f);
            Opacity *= fadeFactor;
            Squish.X *= fadeFactor;

            if (AttachedProjectile != null && AttachedProjectile.active)
                Position = AttachedProjectile.Center + Vector2.UnitY * DisplacementFromProjectile;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = GeneralParticleHandler.GetTexture(Type);

            Vector2 Shake = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (1 - (Time / (float)Lifetime)) * ShakePower;

            //The draw happens at the base of the texture
            Vector2 Origin = new Vector2(tex.Width / 2f, tex.Height);

            Color drawColor = Color.Lerp(Color.White, Color, (Time / (float)Lifetime));

            SpriteEffects flip = (Main.GlobalTime % 30 < 15) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(tex, Position + Shake - Main.screenPosition, null, Color * Opacity * 0.6f, Rotation, Origin, Squish * Scale, flip, 0);
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, drawColor * Opacity, Rotation, Origin, Squish * Scale, flip, 0);
        }
    }
}

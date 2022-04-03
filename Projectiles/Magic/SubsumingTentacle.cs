using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SubsumingTentacle : ModProjectile
    {
        public int AlphaFade
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public Vector2 OffsetAcceleration
        {
            get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
            }
        }
        public const float SegmentOffset = 5f;
        public const float MaxArcingSpeed = 16f;
        public const float MaxHomingSpeed = 18f;
        public const float MaxEnemyDistance = 1450f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Tentacle");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 75;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            if (!Projectile.tileCollide)
            {
                AlphaFade++;
                Projectile.alpha = AlphaFade;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                    return;
                }
            }
            // Here, the old positions act more like "control points" than old postions, and will be referred as such henceforth.
            // Each control point should have a set offset, to give a "chain" effect.
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1] + Vector2.Normalize(Projectile.oldPos[i] - Projectile.oldPos[i - 1]) * SegmentOffset;
            }
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)(20f * Projectile.scale));
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height) && Projectile.tileCollide)
            {
                Projectile.tileCollide = false;
            }

            NPC closestTarget = Projectile.Center.ClosestNPCAt(MaxEnemyDistance, true, true);
            if (closestTarget != null)
            {
                HomingMovement(closestTarget);
            }
            else
            {
                ArcingMovement();
            }
            Projectile.scale -= closestTarget is null ? 0.007f : 0.004f;
            if (Projectile.scale <= 0.05f)
            {
                Projectile.Kill();
            }
        }
        public void ArcingMovement()
        {
            // Cause the tentacle to arc around at an increasingly fast rate.
            Projectile.velocity += OffsetAcceleration;
            if (Projectile.velocity.Length() > MaxArcingSpeed)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= MaxArcingSpeed;
            }

            // Accelerate the arc.
            OffsetAcceleration *= 1.035f;
        }
        public void HomingMovement(NPC closestTarget)
        {
            float angleOffset = MathHelper.WrapAngle(Projectile.AngleTo(closestTarget.Center) - Projectile.velocity.ToRotation());
            angleOffset = MathHelper.Clamp(angleOffset, -0.2f, 0.2f);

            if (Projectile.Distance(closestTarget.Center) > 65f)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * MaxHomingSpeed;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            if (Projectile.scale < 1f)
            {
                for (int i = 10; i < Projectile.oldPos.Length; i++)
                {
                    var tentacleShader = GameShaders.Misc["CalamityMod:SubsumingTentacle"];
                    tentacleShader.UseImage("Images/Misc/Perlin");

                    Vector2 drawPos = Projectile.oldPos[i] + ModContent.Request<Texture2D>(Texture).Size() / 2f - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
                    float scale = MathHelper.Lerp(0.05f, 1.3f, i / (float)Projectile.oldPos.Length) * Projectile.scale;
                    scale = MathHelper.Clamp(scale, 0f, 2f);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / Projectile.oldPos.Length);

                    Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture), drawPos, null, color, Projectile.rotation, ModContent.Request<Texture2D>(Texture).Size() / 2f, scale, SpriteEffects.None, 0);
                    tentacleShader.UseSaturation(i / (float)Projectile.oldPos.Length); // A "completion ratio" for the shader. Used to make the entire tentacle appear multi-colored.
                    tentacleShader.UseOpacity(1f / Projectile.oldPos.Length); // A "step value" for the shader. Used to give variance in color at each individual segment.
                    tentacleShader.Apply(null);
                }
            }
            return false;
        }
        public override void PostDraw(Color drawColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float scale = MathHelper.Lerp(0.05f, 1f, i / (float)Projectile.oldPos.Length) * Projectile.scale * 0.85f;
                if (targetHitbox.Intersects(new Rectangle((int)Projectile.oldPos[i].X, (int)Projectile.oldPos[i].Y, (int)(Projectile.width * scale), (int)(Projectile.height * scale))))
                    return true;
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            return false;
        }
    }
}

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
            get => (int)projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public Vector2 OffsetAcceleration
        {
            get => new Vector2(projectile.ai[0], projectile.ai[1]);
            set
            {
                projectile.ai[0] = value.X;
                projectile.ai[1] = value.Y;
            }
        }
        public const float SegmentOffset = 5f;
        public const float MaxArcingSpeed = 16f;
        public const float MaxHomingSpeed = 18f;
        public const float MaxEnemyDistance = 1450f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Tentacle");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 75;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.MaxUpdates = 2;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            if (!projectile.tileCollide)
            {
                AlphaFade++;
                projectile.alpha = AlphaFade;
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                    return;
                }
            }
            // Here, the old positions act more like "control points" than old postions, and will be referred as such henceforth.
            // Each control point should have a set offset, to give a "chain" effect.
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                projectile.oldPos[i] = projectile.oldPos[i - 1] + Vector2.Normalize(projectile.oldPos[i] - projectile.oldPos[i - 1]) * SegmentOffset;
            }
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(20f * projectile.scale));
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height) && projectile.tileCollide)
            {
                projectile.tileCollide = false;
            }

            NPC closestTarget = projectile.Center.ClosestNPCAt(MaxEnemyDistance, true, true);
            if (closestTarget != null)
            {
                HomingMovement(closestTarget);
            }
            else
            {
                ArcingMovement();
            }
            projectile.scale -= closestTarget is null ? 0.007f : 0.004f;
            if (projectile.scale <= 0.05f)
            {
                projectile.Kill();
            }
        }
        public void ArcingMovement()
        {
            // Cause the tentacle to arc around at an increasingly fast rate.
            projectile.velocity += OffsetAcceleration;
            if (projectile.velocity.Length() > MaxArcingSpeed)
            {
                projectile.velocity.Normalize();
                projectile.velocity *= MaxArcingSpeed;
            }

            // Accelerate the arc.
            OffsetAcceleration *= 1.035f;
        }
        public void HomingMovement(NPC closestTarget)
        {
            float angleOffset = MathHelper.WrapAngle(projectile.AngleTo(closestTarget.Center) - projectile.velocity.ToRotation());
            angleOffset = MathHelper.Clamp(angleOffset, -0.2f, 0.2f);

            if (projectile.Distance(closestTarget.Center) > 65f)
            {
                projectile.velocity = projectile.velocity.RotatedBy(angleOffset);
                projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * MaxHomingSpeed;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            if (projectile.scale < 1f)
            {
                for (int i = 10; i < projectile.oldPos.Length; i++)
                {
                    var tentacleShader = GameShaders.Misc["CalamityMod:SubsumingTentacle"];
                    tentacleShader.UseImage("Images/Misc/Perlin");

                    Vector2 drawPos = projectile.oldPos[i] + ModContent.GetTexture(Texture).Size() / 2f - Main.screenPosition + projectile.gfxOffY * Vector2.UnitY;
                    float scale = MathHelper.Lerp(0.05f, 1.3f, i / (float)projectile.oldPos.Length) * projectile.scale;
                    scale = MathHelper.Clamp(scale, 0f, 2f);
                    Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);

                    spriteBatch.Draw(ModContent.GetTexture(Texture), drawPos, null, color, projectile.rotation, ModContent.GetTexture(Texture).Size() / 2f, scale, SpriteEffects.None, 0f);
                    tentacleShader.UseSaturation(i / (float)projectile.oldPos.Length); // A "completion ratio" for the shader. Used to make the entire tentacle appear multi-colored.
                    tentacleShader.UseOpacity(1f / projectile.oldPos.Length); // A "step value" for the shader. Used to give variance in color at each individual segment.
                    tentacleShader.Apply(null);
                }
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                float scale = MathHelper.Lerp(0.05f, 1f, i / (float)projectile.oldPos.Length) * projectile.scale * 0.85f;
                if (targetHitbox.Intersects(new Rectangle((int)projectile.oldPos[i].X, (int)projectile.oldPos[i].Y, (int)(projectile.width * scale), (int)(projectile.height * scale))))
                    return true;
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            return false;
        }
    }
}

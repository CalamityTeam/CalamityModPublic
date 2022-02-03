using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class MythrilFlare : ModProjectile
    {
        public PrimitiveTrail FlameTrailDrawer = null;
        public ref float Time => ref projectile.ai[0];
        public const int AttackDelay = 22;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mythril Flare");
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = projectile.MaxUpdates * 210;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Create a puff of energy on the first frame.
            if (projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust energyPuff = Dust.NewDustPerfect(projectile.Center, 267);
                    energyPuff.velocity = -Vector2.UnitY.RotatedByRandom(0.81f) * Main.rand.NextFloat(1.25f, 6f);
                    energyPuff.color = Color.Lerp(new Color(104, 183, 136), Color.White, Main.rand.NextFloat(0.5f));
                    energyPuff.scale = 1.1f;
                    energyPuff.alpha = 185;
                    energyPuff.noGravity = true;
                }
                projectile.localAI[0] = 1f;
            }

            NPC potentialTarget = projectile.Center.ClosestNPCAt(176f);

            // Increment the timer on the last extra update.
            if (projectile.FinalExtraUpdate())
                Time++;

            // Sit in place for a moment.
            if (Time < AttackDelay)
                projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.Pi / 8f) * 5f;

            // Rapidly move towards the nearest target.
            if (potentialTarget != null)
            {
                float distanceFromTarget = projectile.Distance(potentialTarget.Center);
                float moveInterpolant = Utils.InverseLerp(0f, 100f, distanceFromTarget, true) * Utils.InverseLerp(600f, 400f, distanceFromTarget, true);
                Vector2 targetCenterOffsetVec = potentialTarget.Center - projectile.Center;
                float movementSpeed = MathHelper.Min(17.5f, targetCenterOffsetVec.Length());
                Vector2 idealVelocity = targetCenterOffsetVec.SafeNormalize(Vector2.Zero) * movementSpeed;

                // Die if anything goes wrong with the velocity.
                if (projectile.velocity.HasNaNs())
                    projectile.Kill();

                // Approach the ideal velocity.
                projectile.velocity = Vector2.Lerp(projectile.velocity, idealVelocity, moveInterpolant * 0.08f);
                projectile.velocity = projectile.velocity.MoveTowards(idealVelocity, 2f);
            }
        }

        public override bool CanDamage() => Time >= AttackDelay;

        public Color TrailColor(float completionRatio)
        {
            float trailOpacity = Utils.InverseLerp(0f, 0.13f, completionRatio, true) * Utils.InverseLerp(0.7f, 0.58f, completionRatio, true);
            Color startingColor = Color.Lerp(Color.White, Color.LightGreen, 0.47f);
            Color middleColor = Color.LightGreen;
            Color endColor = Color.Transparent;
            return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * trailOpacity;
        }

        public float TrailWidth(float completionRatio) => MathHelper.SmoothStep(projectile.width, 4.25f, completionRatio);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (FlameTrailDrawer is null)
                FlameTrailDrawer = new PrimitiveTrail(TrailWidth, TrailColor, null, GameShaders.Misc["CalamityMod:ImpFlameTrail"]);

            // Prepare the flame trail shader with its map texture.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/EternityStreak"));
            FlameTrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 74);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust energyPuff = Dust.NewDustPerfect(projectile.Center, 267);
                energyPuff.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.97f) * Main.rand.NextFloat(1f, 8f);
                energyPuff.color = Color.Lerp(new Color(104, 183, 136), Color.White, Main.rand.NextFloat(0.5f));
                energyPuff.scale = 1.1f;
                energyPuff.alpha = 185;
                energyPuff.noGravity = true;
            }
        }
    }
}

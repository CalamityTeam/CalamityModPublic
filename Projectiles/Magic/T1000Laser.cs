using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class T1000Laser : ModProjectile
    {
        private const float LaserLength = 80f;
        private const float LaserLengthChangeRate = 2f;
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phased God Ray");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Very rapidly fade into existence.
            if (projectile.alpha > 0)
                projectile.alpha -= 25;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            float theta = 0.09f;
            float maxWaveFrames = 9f;
            float waveSign = projectile.localAI[1] < 0f ? -1f : 1f;

            // Choose a waving direction initially at random.
            if (projectile.localAI[1] == 0f)
            {
                float coinflip = Main.rand.NextBool() ? -1f : 1f;
                waveSign = -coinflip;
                projectile.localAI[1] = coinflip * maxWaveFrames * 0.5f;

                // Backfill old rotations to prevent visual glitches.
                float startingRotation = projectile.velocity.ToRotation();
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                    projectile.oldRot[i] = startingRotation;

                // Adjust initial position to prevent ugly splitting.
                Vector2 perp = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2);
                float distance = Main.rand.NextFloat(13f, 30f);
                projectile.position -= coinflip * distance * perp;
            }
            // Switch waving directions as necessary.
            else if (Math.Abs(projectile.localAI[1]) > maxWaveFrames)
                projectile.localAI[1] = -waveSign;
            else
                projectile.localAI[1] += waveSign;

            // Apply a constant, rapid wave to the laser's motion.
            projectile.velocity = projectile.velocity.RotatedBy(waveSign * theta);
            projectile.rotation = projectile.velocity.ToRotation();

            // Emit light.
            Lighting.AddLight(projectile.Center, 0.87f, 0.65f, 0.1725f);

            // Laser length shenanigans. If the laser is still growing, increase localAI 0 to indicate it is getting longer.
            if (projectile.ai[1] == 0f)
            {
                projectile.localAI[0] += 10f; // LaserLengthChangeRate;

                // Cap it at max length.
                if (projectile.localAI[0] > LaserLength)
                    projectile.localAI[0] = LaserLength;
            }

            // Otherwise it's shrinking. Once it reaches zero length it dies for good.
            else
            {
                projectile.localAI[0] -= LaserLengthChangeRate;
                if (projectile.localAI[0] <= 0f)
                    projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Get an extra hit in despite not being officially piercing.
            if (projectile.ai[0] == 0f)
            {
                ++projectile.penetrate;
                projectile.ai[0] += 1f;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(222, 166, 44, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(LaserLength, 2f, lightColor, curve: true);

        public override void Kill(int timeLeft)
        {
            int dustID = 269;
            int dustAmt = 4;
            Vector2 dustPos = projectile.Center - projectile.velocity / 2f;
            Vector2 dustVel = projectile.velocity / 4f;
            for (int i = 0; i < dustAmt; ++i)
            {
                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f, Scale: 2.5f);
                d.velocity += dustVel;
                d.velocity *= Main.rand.NextFloat(0.4f, 1f);
                d.noGravity = true;
            }
        }
    }
}

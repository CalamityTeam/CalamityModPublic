using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TelluricGlareArrow : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public PrimitiveTrail TrailDrawer = null;
        private const int Lifetime = 180;
        private static Color ShaderColorOne = new Color(237, 194, 66);
        private static Color ShaderColorTwo = new Color(235, 227, 117);
        private static Color ShaderEndColor = new Color(199, 153, 26);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Arrow");
            // While this projectile doesn't have afterimages, it keeps track of old positions for its primitive drawcode.
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 21;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 24;
            projectile.arrow = true;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = Lifetime;
            projectile.MaxUpdates = 3;
            projectile.penetrate = 2; // Can hit up to two enemies. Will explode extremely soon after hitting the first, though.
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool CanDamage() => projectile.timeLeft < Lifetime - 4;

        public override void AI() => Lighting.AddLight(projectile.Center, ShaderColorOne.ToVector3());

        private void RestrictLifetime()
        {
            if (projectile.timeLeft > 8)
                projectile.timeLeft = 8;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            RestrictLifetime();
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            RestrictLifetime();
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        private float PrimitiveWidthFunction(float completionRatio)
        {
            float arrowheadCutoff = 0.36f;
            float width = 39f;
            float minHeadWidth = 0.02f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.InverseLerp(0f, arrowheadCutoff, completionRatio, true));
            return width;
        }

        private Color PrimitiveColorFunction(float completionRatio)
        {
            float endFadeRatio = 0.41f;

            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.InverseLerp(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTime * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);

            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(0f, endFadeRatio, completionRatio, true)));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/TelluricGlareStreak"));
            Vector2 overallOffset = projectile.Size * 0.5f - Main.screenPosition;
            overallOffset += projectile.velocity * 1.4f;
            TrailDrawer.Draw(projectile.oldPos, overallOffset, 92); // 58
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);

            // Explode into a bunch of holy fire on death.
            for (int i = 0; i < 10; i++)
            {
                Dust holyFire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                holyFire.velocity *= 3f;

                if (Main.rand.NextBool(2))
                {
                    holyFire.scale = 0.5f;
                    holyFire.fadeIn = Main.rand.NextFloat(1f, 2f);
                }
            }
            for (int i = 0; i < 20; i++)
            {
                Dust holyFire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, default, 3f);
                holyFire.noGravity = true;
                holyFire.velocity *= 5f;

                holyFire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                holyFire.velocity *= 2f;
            }
        }
    }
}

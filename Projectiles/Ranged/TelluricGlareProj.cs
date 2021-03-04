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
    public class TelluricGlareProj : ModProjectile
    {
        public PrimitiveTrail TrailDrawer = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glare");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 11;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 14;
            projectile.arrow = true;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.aiStyle = 1;
            projectile.timeLeft = 180;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public float PrimitiveWidthFunction(float completionRatio)
		{
            float width;
            if (completionRatio <= 0.23f)
                width = MathHelper.SmoothStep(1f, projectile.width * 1.4f, Utils.InverseLerp(0f, 0.23f, completionRatio, true));
            else
            {
                width = MathHelper.SmoothStep(projectile.width * 2f, projectile.width, Utils.InverseLerp(0.18f, 0.29f, completionRatio, true));
                width *= Utils.InverseLerp(1f, 0.51f, completionRatio, true);
            }

            return width;
        }

        public Color PrimitiveColorFunction(float completionRatio)
		{
            float whiteFadeRequirement = 0.41f;
            float startingInterpolant = (float)Math.Cos(completionRatio * 2.7f - Main.GlobalTime * 5.3f + Utils.InverseLerp(0f, whiteFadeRequirement * 0.5f, completionRatio, true) * 3.2f);
            startingInterpolant = startingInterpolant * 0.5f + 0.5f;
            Color startingColor = Color.Lerp(Color.Yellow, Color.OrangeRed, startingInterpolant * 0.6f);
            return Color.Lerp(startingColor, Color.Wheat, MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(0f, whiteFadeRequirement, completionRatio, true)));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/TelluricGlareStreak"));
            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 58);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 36);
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }
}

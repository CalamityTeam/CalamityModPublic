using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ViolenceThrownProjectile : ModProjectile
    {
        internal PrimitiveTrail StreakDrawer = null;
        internal Player Owner => Main.player[projectile.owner];
        internal ref float Time => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Violence";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Violence");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 142;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 90000;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
			projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Fade in.
            projectile.Opacity = Utils.InverseLerp(0f, 15f, Time, true);

            if (Owner.channel)
            {
                HomeTowardsMouse();
                projectile.rotation += 0.35f / projectile.MaxUpdates;
            }
            else
            {
                ReturnToOwner();

                float idealAngle = projectile.AngleTo(Owner.Center) - MathHelper.PiOver4 + MathHelper.Pi;
                projectile.rotation = projectile.rotation.AngleLerp(idealAngle, 0.1f);
                projectile.rotation = projectile.rotation.AngleTowards(idealAngle, 0.25f);
            }
            ManipulatePlayerFields();

            // Create some demonic light at the tip of the spear.
            Lighting.AddLight(projectile.Center + (projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * projectile.height * 0.45f, Color.Red.ToVector3() * 0.4f);
            Time++;
        }

        internal void HomeTowardsMouse()
        {
            if (Main.myPlayer != projectile.owner)
                return;

            if (projectile.WithinRange(Main.MouseWorld, projectile.velocity.Length() * 0.7f))
                projectile.Center = Main.MouseWorld;
            else
                projectile.velocity = (projectile.velocity * 6f + projectile.DirectionTo(Main.MouseWorld) * 14f) / 7f;
            projectile.netSpam = 0;
            projectile.netUpdate = true;
        }

        internal void ReturnToOwner()
        {
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center, 0.02f);
            projectile.velocity = projectile.DirectionTo(Owner.Center) * 15f;
            if (projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                for (int i = 0; i < 75; i++)
                {
                    Dust fire = Dust.NewDustPerfect(Owner.Center, DustID.Fire);
                    fire.velocity = (MathHelper.TwoPi * i / 75f).ToRotationVector2() * 4f - Vector2.UnitY * 3f;
                    fire.scale = 1.4f;
                    fire.noGravity = true;
                }
                projectile.Kill();
            }
        }

        internal void ManipulatePlayerFields()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        internal float PrimitiveWidthFunction(float completionRatio)
        {
            float tipWidthFactor = MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(0.01f, 0.04f, completionRatio));
            float bodyWidthFactor = (float)Math.Pow(Utils.InverseLerp(1f, 0.04f, completionRatio), 0.9D);
            return (float)Math.Pow(tipWidthFactor * bodyWidthFactor, 0.1D) * 30f;
        }

        internal Color PrimitiveColorFunction(float completionRatio)
        {
            float fadeInterpolant = (float)Math.Cos(Main.GlobalTime * -9f + completionRatio * 6f + projectile.identity * 2f) * 0.5f + 0.5f;

            // Adjust the fade interpolant to be on a different scale via a linear interpolation.
            fadeInterpolant = MathHelper.Lerp(0.15f, 0.75f, fadeInterpolant);
            Color frontFade = Color.Lerp(new Color(255, 145, 115), new Color(113, 0, 159), fadeInterpolant);

            // Go halfway to a dark red color.
            frontFade = Color.Lerp(frontFade, Color.DarkRed, 0.5f);
            Color backFade = new Color(255, 145, 115);

            return Color.Lerp(frontFade, backFade, (float)Math.Pow(completionRatio, 1.2D)) * (float)Math.Pow(1f - completionRatio, 1.1D) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (StreakDrawer is null)
                StreakDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/FabstaffStreak"));

            Texture2D spearProjectile = Main.projectileTexture[projectile.type];

            // Not cloning the points causes the below operations to be applied to the original oldPos value by reference
            // and thus causes it to be consistently added over and over, which is not intended behavior.
            Vector2[] drawPoints = (Vector2[])projectile.oldPos.Clone();
            Vector2 aimAheadDirection = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();

            if (Owner.channel)
            {
                drawPoints[0] += aimAheadDirection * -12f;
                drawPoints[1] = drawPoints[0] - (projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * Vector2.Distance(drawPoints[0], drawPoints[1]);
            }
            for (int i = 0; i < drawPoints.Length; i++)
                drawPoints[i] -= (projectile.oldRot[i] + MathHelper.PiOver4).ToRotationVector2() * projectile.height * 0.5f;

            if (Time > projectile.oldPos.Length)
                StreakDrawer.Draw(drawPoints, projectile.Size * 0.5f - Main.screenPosition, 88);

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            for (int i = 0; i < 6; i++)
            {
                float rotation = projectile.oldRot[i] - MathHelper.PiOver2;
                if (Owner.channel)
                    rotation += 0.2f;

                Color afterimageColor = Color.Lerp(lightColor, Color.Transparent, 1f - (float)Math.Pow(Utils.InverseLerp(0, 6, i), 1.4D)) * projectile.Opacity;
                spriteBatch.Draw(spearProjectile, drawPosition, null, afterimageColor, rotation, spearProjectile.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}

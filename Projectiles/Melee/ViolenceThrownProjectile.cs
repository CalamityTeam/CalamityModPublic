using CalamityMod.Particles;
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
                projectile.rotation += 0.45f / projectile.MaxUpdates;
            }
            else
            {
                ReturnToOwner();

                float idealAngle = projectile.AngleTo(Owner.Center) - MathHelper.PiOver4;
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
                projectile.velocity = (projectile.velocity * 3f + projectile.DirectionTo(Main.MouseWorld) * 19f) / 4f;
            projectile.netSpam = 0;
            projectile.netUpdate = true;
        }

        internal void ReturnToOwner()
        {
            projectile.Center = Vector2.Lerp(projectile.Center, Owner.Center, 0.02f);
            projectile.velocity = projectile.SafeDirectionTo(Owner.Center) * 22f;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                // Play a splatter and impact sound.
                Main.PlaySound(SoundID.DD2_CrystalCartImpact, projectile.Center);

                float damageInterpolant = Utils.InverseLerp(950f, 2000f, damage, true);
                float impactAngularVelocity = MathHelper.Lerp(0.08f, 0.2f, damageInterpolant);
                float impactParticleScale = MathHelper.Lerp(0.6f, 1f, damageInterpolant);
                impactAngularVelocity *= Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(0.75f, 1.25f);

                Color impactColor = Color.Lerp(Color.Silver, Color.Gold, Main.rand.NextFloat(0.5f));
                Vector2 impactPoint = Vector2.Lerp(projectile.Center, target.Center, 0.65f);
                Vector2 bloodSpawnPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height) * 0.04f;
                Vector2 splatterDirection = (projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);

                // Emit blood if the target is organic.
                if (target.Organic())
                {
                    Main.PlaySound(SoundID.NPCHit18, projectile.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        int bloodLifetime = Main.rand.Next(22, 36);
                        float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                        Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                        bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                        if (Main.rand.NextBool(20))
                            bloodScale *= 2f;

                        Vector2 bloodVelocity = splatterDirection.RotatedByRandom(0.81f) * Main.rand.NextFloat(11f, 23f);
                        bloodVelocity.Y -= 12f;
                        BloodParticle blood = new BloodParticle(bloodSpawnPosition, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                        GeneralParticleHandler.SpawnParticle(blood);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        float bloodScale = Main.rand.NextFloat(0.2f, 0.33f);
                        Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0.5f, 1f));
                        Vector2 bloodVelocity = splatterDirection.RotatedByRandom(0.9f) * Main.rand.NextFloat(9f, 14.5f);
                        BloodParticle2 blood = new BloodParticle2(bloodSpawnPosition, bloodVelocity, 20, bloodScale, bloodColor);
                        GeneralParticleHandler.SpawnParticle(blood);
                    }
                }

                // Emit sparks if the target is not organic.
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int sparkLifetime = Main.rand.Next(22, 36);
                        float sparkScale = Main.rand.NextFloat(0.8f, 1f) + damageInterpolant * 0.85f;
                        Color sparkColor = Color.Lerp(Color.Silver, Color.Gold, Main.rand.NextFloat(0.7f));
                        sparkColor = Color.Lerp(sparkColor, Color.Orange, Main.rand.NextFloat());

                        if (Main.rand.NextBool(10))
                            sparkScale *= 2f;

                        Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 25f);
                        sparkVelocity.Y -= 6f;
                        SparkParticle spark = new SparkParticle(impactPoint, sparkVelocity, true, sparkLifetime, sparkScale, sparkColor);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }

                // And create an impact point particle.
                ImpactParticle impactParticle = new ImpactParticle(impactPoint, impactAngularVelocity, 20, impactParticleScale, impactColor);
                GeneralParticleHandler.SpawnParticle(impactParticle);
            }
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

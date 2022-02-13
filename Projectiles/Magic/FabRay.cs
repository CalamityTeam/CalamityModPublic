using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class FabRay : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ray");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 54;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 70;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 60 * projectile.extraUpdates;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.2f, 0.01f, 0.1f);
            projectile.ai[0] += 1f;
            projectile.Opacity = Utils.InverseLerp(0f, 10f * projectile.MaxUpdates, projectile.timeLeft, true);

            int shootRate = projectile.npcProj ? 40 : 12;
            if (projectile.owner == Main.myPlayer && projectile.ai[0] % shootRate == 0f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(180f, false);
                if (potentialTarget != null)
                {
                    Vector2 shootVelocity = projectile.SafeDirectionTo(potentialTarget.Center) * 13f;
                    Projectile.NewProjectile(projectile.Center - shootVelocity * 2.5f, shootVelocity, ModContent.ProjectileType<FabBolt>(), projectile.damage, projectile.knockBack, projectile.owner);
                }

                for (int i = 0; i < projectile.oldPos.Length / 4; i += 3)
                {
                    potentialTarget = projectile.oldPos[i].ClosestNPCAt(280f, false);
                    if (potentialTarget != null)
                    {
                        Vector2 shootVelocity = (potentialTarget.Center - projectile.oldPos[i]).SafeNormalize(Vector2.UnitY) * 13f;
                        Projectile.NewProjectile(projectile.oldPos[i] - shootVelocity * 2.5f, shootVelocity, ModContent.ProjectileType<FabBolt>(), projectile.damage, projectile.knockBack, projectile.owner);
                        break;
                    }
                }
            }

            // Emit light.
            Lighting.AddLight(projectile.Center, Vector3.One * projectile.Opacity * 0.7f);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-Main.GlobalTime * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.InverseLerp(1f, 0.64f, completionRatio, true) * projectile.Opacity;
            Color endColor = Color.Lerp(Color.Cyan, Color.HotPink, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTime * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.InverseLerp(0f, 0.3f, completionRatio, true), 2D);
            return MathHelper.Lerp(0f, 32f * projectile.Opacity, expansionCompletion);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
            {
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"])
                {
                    DegreeOfBezierCurveCornerSmoothening = 4
                };
            }

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));
            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 32);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
    }
}

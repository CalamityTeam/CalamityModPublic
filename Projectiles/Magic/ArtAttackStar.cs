using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ArtAttackStar : ModProjectile
    {
        public PrimitiveTrail TrailDrawer = null;
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public const int StarShapeCreationDelay = 12;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 120;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 9000;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 60;

            // Die if the holdout is gone.
            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<ArtAttackHoldout>()] <= 0 && Time >= 2f)
            {
                projectile.Kill();
                return;
            }

            bool shapeIsComplete = false;
            int shapeEndPoint = -1;

            // Determine if two points are intersecting with the star.
            List<Vector2> cleanOldPositions = projectile.oldPos.Where(p => p != Vector2.Zero).ToList();
            if (Time > StarShapeCreationDelay)
            {
                int start = 12;
                int end = cleanOldPositions.Count;
                float averageDistanceFromStar = 0f;
                float closedAngleLowerBound = (cleanOldPositions.Count - 2f) * MathHelper.Pi;
                for (int i = end - 1; i >= start; i--)
                {
                    float distanceFromStar = Vector2.Distance(projectile.position, projectile.oldPos[i]);
                    if (distanceFromStar < projectile.velocity.Length() * 0.7f + 12f)
                    {
                        shapeIsComplete = true;

                        if (shapeEndPoint == -1)
                            shapeEndPoint = i;
                    }
                    averageDistanceFromStar += distanceFromStar;
                }
                averageDistanceFromStar /= end - start;

                // Cancel out intersection "completions" if the velocity is slow enough to be rebounding or the shape is relatively small.
                if (averageDistanceFromStar < projectile.velocity.Length() + 70f || projectile.velocity.Length() < 16f)
                    shapeIsComplete = false;
            }

            // Idly emit dust.
            EmitIdleDust();

            // Die if the owner is no longer channeling the staff or a complete shape is made.
            // If a complete shape is made, do damage checks.
            // The shape end point exists in cases where a shape is completed but an NPC is not in the part that is enclosed.
            if (!Owner.channel || shapeIsComplete)
            {
                if (shapeIsComplete)
                {
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact, Owner.Center);
                    DoShapeHitAreaChecks(cleanOldPositions, shapeEndPoint);
                }

                projectile.Kill();
                return;
            }

            // Fade in.
            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.1f, 0f, 1f);

            // Update movement.
            if (Main.myPlayer == projectile.owner)
                DoMouseMovement();

            Time++;
        }

        public void DoMouseMovement()
        {
            Vector2 destination = Main.MouseWorld;
            float distanceFromTarget = projectile.Distance(destination);
            float moveInterpolant = Utils.InverseLerp(0f, 100f, distanceFromTarget, true) * Utils.InverseLerp(600f, 400f, distanceFromTarget, true);
            Vector2 targetCenterOffsetVec = destination - projectile.Center;
            float movementSpeed = MathHelper.Min(60f, targetCenterOffsetVec.Length());
            Vector2 idealVelocity = targetCenterOffsetVec.SafeNormalize(Vector2.Zero) * movementSpeed;

            // Ensure velocity never has a magnitude less than 2.
            if (projectile.velocity.Length() < 2f)
                projectile.velocity += projectile.velocity.RotatedBy(MathHelper.PiOver4).SafeNormalize(Vector2.Zero) * 2f;

            // Die if anything goes wrong with the velocity.
            if (projectile.velocity.HasNaNs())
                projectile.Kill();

            // Approach the ideal velocity.
            projectile.velocity = Vector2.Lerp(projectile.velocity, idealVelocity, moveInterpolant * 0.15f);
            projectile.velocity = projectile.velocity.MoveTowards(idealVelocity, 6f);
            if (projectile.velocity.AngleBetween(projectile.oldVelocity) < 0.85f && projectile.velocity.AngleBetween(idealVelocity) > 1.4f)
                projectile.velocity = projectile.velocity.RotatedBy(0.35f) * 0.75f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Continuously sync since mouse information is local.
            projectile.netUpdate = true;
            projectile.netSpam = 0;
        }

        public void EmitIdleDust()
        {
            bool slowMovement = projectile.velocity.Length() < 6f;
            int dustCount = slowMovement ? 3 : 1;

            for (int i = 0; i < dustCount; i++)
            {
                Dust rainbowMagic = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(8f, 8f), 261);
                rainbowMagic.velocity = Main.rand.NextVector2Circular(6f, 6f) - (projectile.velocity * 0.16f).RotatedByRandom(0.51f);
                if (slowMovement)
                    rainbowMagic.velocity -= Vector2.UnitY.RotatedByRandom(0.81f) * Main.rand.NextFloat(4.5f);

                rainbowMagic.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, Main.rand.NextFloat(0.5f, 0.9f));
                rainbowMagic.color.A = 128;
                rainbowMagic.scale = Main.rand.NextFloat(1.3f, 1.6f);
                rainbowMagic.fadeIn = 0.4f;
                rainbowMagic.noGravity = true;
            }
        }

        public void DoShapeHitAreaChecks(List<Vector2> cleanOldPositions, int shapeEndPoint)
        {
            float damageFactor = MathHelper.Lerp(1f, ArtAttack.MaxDamageBoostFactor, Utils.InverseLerp(0f, ArtAttack.MaxDamageBoostTime, Time, true));
            int damage = (int)(projectile.damage * damageFactor);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (!Main.npc[i].CanBeChasedBy())
                    continue;

                bool enemyIsInShape = false;

                // Find the top left and bottom right bounds of the shape by using min/maxes.
                Vector2 topLeft = new Vector2(1000000f, 1000000f);
                Vector2 bottomRight = new Vector2(-1000000f, -1000000f);
                for (int j = 0; j < shapeEndPoint; j++)
                {
                    topLeft.X = MathHelper.Min(topLeft.X, cleanOldPositions[j].X);
                    topLeft.Y = MathHelper.Min(topLeft.Y, cleanOldPositions[j].Y);
                    bottomRight.X = MathHelper.Max(bottomRight.X, cleanOldPositions[j].X);
                    bottomRight.Y = MathHelper.Max(bottomRight.Y, cleanOldPositions[j].Y);
                }

                // Roughly estimate if the NPC is within the shape with line v line and rectangle checks.
                // There's probably a more mathematically pleasing way to do this but for it works sufficiently well.
                Vector2 center = (topLeft + bottomRight) * 0.5f;
                Vector2 area = new Vector2(Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y)) * 0.8f;
                Rectangle shapeRectangle = Utils.CenteredRectangle(center, area);
                for (int j = 0; j < cleanOldPositions.Count; j++)
                {
                    Vector2 left = Main.npc[i].Center - Vector2.UnitX * 2000f;
                    Vector2 right = Main.npc[i].Center + Vector2.UnitX * 2000f;
                    bool inRangeOfStars = shapeRectangle.Intersects(Main.npc[i].Hitbox);
                    bool lineCheck = Collision.CheckLinevLine(left, right, cleanOldPositions[j], cleanOldPositions[(j + 1) % cleanOldPositions.Count]).Length > 0;
                    if (lineCheck && inRangeOfStars)
                    {
                        enemyIsInShape = true;
                        break;
                    }
                }

                // Strike an enemy if it's in the shape.
                if (enemyIsInShape)
                {
                    Main.PlaySound(SoundID.DD2_LightningBugZap, Main.npc[i].Center);
                    CreateDustExplosionEffect(Main.npc[i].Center);

                    if (Main.myPlayer == projectile.owner)
                    {
                        int strike = Projectile.NewProjectile(Main.npc[i].Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, projectile.owner, i);
                        if (Main.projectile.IndexInRange(strike))
                            Main.projectile[strike].Calamity().forceMelee = true;
                    }
                }
            }
        }

        public void CreateDustExplosionEffect(Vector2 dustSpawnPosition)
        {
            for (int i = 0; i < 60; i++)
            {
                Dust rainbowMagic = Dust.NewDustPerfect(dustSpawnPosition + Main.rand.NextVector2Circular(12f, 12f), 267);
                rainbowMagic.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 9.5f);
                rainbowMagic.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, Main.rand.NextFloat(0.5f, 0.9f));
                rainbowMagic.color.A = 100;
                rainbowMagic.scale = Main.rand.NextFloat(1f, 1.25f);
                rainbowMagic.fadeIn = Main.rand.NextFloat(0.4f, 1f);
                rainbowMagic.noGravity = true;
            }
        }

        public override void Kill(int timeLeft) => CreateDustExplosionEffect(projectile.Center);

        public Color TrailColor(float completionRatio)
        {
            float hue = (Main.GlobalTime * -0.62f + completionRatio * 1.5f) % 1f;
            float brightness = MathHelper.SmoothStep(0.5f, 1f, Utils.InverseLerp(0.3f, 0f, completionRatio, true));
            float opacity = Utils.InverseLerp(1f, 0.8f, completionRatio, true) * projectile.Opacity;
            Color color = Main.hslToRgb(hue, 1f, brightness) * opacity;
            color.A = (byte)(int)(Utils.InverseLerp(0f, 0.2f, completionRatio) * 128);
            return color;
        }

        public float TrailWidth(float completionRatio)
        {
            float widthInterpolant = Utils.InverseLerp(-0.1f, 0.25f, completionRatio, true) * Utils.InverseLerp(1.1f, 0.5f, completionRatio, true);
            return MathHelper.SmoothStep(0f, 20f, widthInterpolant);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(TrailWidth, TrailColor, null, GameShaders.Misc["CalamityMod:ArtAttack"]);

            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPosition = projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
            Vector2 origin = texture.Size() * 0.5f;

            spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/FabstaffStreak"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();

            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 100);
            spriteBatch.ExitShaderRegion();

            spriteBatch.Draw(texture, drawPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, origin, projectile.scale, 0, 0f);
            return false;
        }

        // The star itself does not do contact damage.
        public override bool CanDamage() => false;
    }
}

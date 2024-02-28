using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ArtAttackStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public const int StarShapeCreationDelay = 12;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 180;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9000;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {

            // Die if the holdout is gone.
            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<ArtAttackHoldout>()] <= 0 && Time >= 2f)
            {
                Projectile.Kill();
                return;
            }

            bool shapeIsComplete = false;
            int shapeEndPoint = -1;
            float distanceTraveled = (Projectile.position - Projectile.oldPos[1]).Length();

            // Determine if two points are intersecting with the star.
            List<Vector2> cleanOldPositions = Projectile.oldPos.Where(p => p != Vector2.Zero).ToList();
            if (Time > StarShapeCreationDelay)
            {
                int start = 12;
                int end = cleanOldPositions.Count;
                float averageDistanceFromStar = 0f;
                for (int i = end - 1; i >= start; i--)
                {
                    float distanceFromStar = Vector2.Distance(Projectile.position, Projectile.oldPos[i]);
                    if (distanceFromStar < (distanceTraveled * 0.7f + 30f)) 
                    {
                        shapeIsComplete = true;

                        if (shapeEndPoint == -1)
                            shapeEndPoint = i;
                    }
                    averageDistanceFromStar += distanceFromStar;
                }
                averageDistanceFromStar /= end - start;

                // Cancel out intersection "completions" if the velocity is slow enough to be rebounding or the shape is relatively small.
                if (averageDistanceFromStar < distanceTraveled + 70f)
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
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Owner.Center);
                    DoShapeHitAreaChecks(cleanOldPositions, shapeEndPoint);
                }

                Projectile.Kill();
                return;
            }

            // Fade in.
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);

            // Update movement.
            if (Main.myPlayer == Projectile.owner)
                DoMouseMovement();

            Time++;
        }

        public void DoMouseMovement()
        {
            Projectile.Center = Main.MouseWorld;

            // Continuously sync since mouse information is local.
            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }

        public void EmitIdleDust()
        {

            for (int i = 0; i < 3; i++)
            {
                Dust rainbowMagic = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8f, 8f), 261);
                rainbowMagic.velocity = Main.rand.NextVector2Circular(6f, 6f) - ((Projectile.position - Projectile.oldPos[1])/3f).RotatedByRandom(0.51f);

                rainbowMagic.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, Main.rand.NextFloat(0.5f, 0.9f));
                rainbowMagic.color.A = 128;
                rainbowMagic.scale = Main.rand.NextFloat(1.3f, 1.6f);
                rainbowMagic.fadeIn = 0.4f;
                rainbowMagic.noGravity = true;
            }
        }

        public void DoShapeHitAreaChecks(List<Vector2> cleanOldPositions, int shapeEndPoint)
        {
            float damageFactor = MathHelper.Lerp(1f, ArtAttack.MaxDamageBoostFactor, Utils.GetLerpValue(0f, ArtAttack.MaxDamageBoostTime, Time, true));
            int damage = (int)(Projectile.damage * damageFactor);
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
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Main.npc[i].Center);
                    CreateDustExplosionEffect(Main.npc[i].Center);

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.npc[i].Center, Vector2.Zero, ModContent.ProjectileType<ArtAttackStrike>(), damage, 0f, Projectile.owner, i);
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

        public override void OnKill(int timeLeft) => CreateDustExplosionEffect(Projectile.Center);

        public Color TrailColor(float completionRatio)
        {
            float hue = (Main.GlobalTimeWrappedHourly * -0.62f + completionRatio * 1.5f) % 1f;
            float brightness = MathHelper.SmoothStep(0.5f, 1f, Utils.GetLerpValue(0.3f, 0f, completionRatio, true));
            float opacity = Utils.GetLerpValue(1f, 0.8f, completionRatio, true) * Projectile.Opacity;
            Color color = Main.hslToRgb(hue, 1f, brightness) * opacity;
            color.A = (byte)(int)(Utils.GetLerpValue(0f, 0.2f, completionRatio) * 128);
            return color;
        }

        public float TrailWidth(float completionRatio)
        {
            float widthInterpolant = Utils.GetLerpValue(0f, 0.25f, completionRatio, true) * Utils.GetLerpValue(1.1f, 0.7f, completionRatio, true);
            return MathHelper.SmoothStep(8f, 20f, widthInterpolant);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = texture.Size() * 0.5f;

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();

            PrimitiveSet.Prepare(Projectile.oldPos, new(TrailWidth, TrailColor, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }

        // The star itself does not do contact damage.
        public override bool? CanDamage() => false;
    }
}

using CalamityMod.Buffs;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorLaserbeam : ModProjectile
    {
        public PrimitiveTrail RayDrawer = null;
        public Player Owner => Main.player[projectile.owner];
        public Projectile MagicCircle
        {
            get
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].identity != projectile.ai[0] || !Main.projectile[i].active || Main.projectile[i].owner != projectile.owner)
                        continue;

                    return Main.projectile[i];
                }
                return null;
            }
        }
        public ref float LaserLength => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float MaxLaserLength = 3330f;

        public override void SetStaticDefaults() => DisplayName.SetDefault("The Angy Beam");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.Calamity().PierceResistHarshness = 0.06f;
            projectile.Calamity().PierceResistCap = 0.4f;
        }

        public override void AI()
        {
            // If the owner is no longer able to cast the beam, kill it.
            if (!Owner.channel || Owner.noItems || Owner.CCed || MagicCircle is null)
            {
                projectile.Kill();
                return;
            }

            // Grow bigger up to a point.
            projectile.scale = MathHelper.Clamp(projectile.scale + 0.15f, 0.05f, 2f);

            // Decide where to position the laserbeam.
            Vector2 circlePointDirection = projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);
            projectile.Center = MagicCircle.Center;

            // Update the laser length.
            float[] laserLengthSamplePoints = new float[24];
            Collision.LaserScan(projectile.Center, projectile.velocity, projectile.scale * 8f, MaxLaserLength, laserLengthSamplePoints);
            LaserLength = laserLengthSamplePoints.Average();

            // Update aim.
            UpdateAim();

            // Adjust damage every frame. This is necessary to ensure that mana sickness and such are applied.
            projectile.damage = (int)(MagicCircle.damage * Owner.MagicDamage());

            // Create arms on surfaces.
            if (Main.myPlayer == projectile.owner && Main.rand.NextBool(8))
                CreateArmsOnSurfaces();

            // Create hit effects at the end of the beam.
            if (Main.myPlayer == projectile.owner)
                CreateTileHitEffects();

            // Make the beam cast light along its length. The brightness of the light is reliant on the scale of the beam.
            DelegateMethods.v3_1 = Color.DarkViolet.ToVector3() * projectile.scale * 0.4f;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, DelegateMethods.CastLight);
        }

        public void UpdateAim()
        {
            // Only execute the aiming code for the owner.
            if (Main.myPlayer != projectile.owner)
                return;

            Vector2 newAimDirection = MagicCircle.velocity.SafeNormalize(Vector2.UnitY);

            // Sync if the direction is different from the old one.
            // Spam caps are ignored due to the frequency of this happening.
            if (newAimDirection != projectile.velocity)
            {
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            projectile.velocity = newAimDirection;
        }

        public void CreateArmsOnSurfaces()
        {
            Vector2 endOfLaser = projectile.Center + projectile.velocity * LaserLength + Main.rand.NextVector2Circular(80f, 8f);
            Vector2 idealCenter = endOfLaser;
            if (WorldUtils.Find(idealCenter.ToTileCoordinates(), Searches.Chain(new Searches.Down(5), new CustomConditions.SolidOrPlatform()), out Point result))
            {
                idealCenter = result.ToWorldCoordinates();
            }
            Point endOfLaserTileCoords = idealCenter.ToTileCoordinates();
            Tile endTile = CalamityUtils.ParanoidTileRetrieval(endOfLaserTileCoords.X, endOfLaserTileCoords.Y);

            if (endTile.nactive() && (Main.tileSolid[endTile.type] || Main.tileSolidTop[endTile.type]) && !endTile.halfBrick() && endTile.slope() == 0)
            {
                Vector2 armSpawnPosition = endOfLaserTileCoords.ToWorldCoordinates();
                Projectile.NewProjectile(armSpawnPosition, Vector2.Zero, ModContent.ProjectileType<RancorArm>(), projectile.damage * 2 / 3, 0f, projectile.owner);
            }
        }

        public void CreateTileHitEffects()
        {
            Vector2 endOfLaser = projectile.Center + projectile.velocity * (LaserLength - Main.rand.NextFloat(12f, 72f));
            Projectile.NewProjectile(endOfLaser, Main.rand.NextVector2Circular(4f, 8f), ModContent.ProjectileType<RancorFog>(), 0, 0f, projectile.owner);

            if (Main.rand.NextBool(2))
            {
                int type = ModContent.ProjectileType<RancorSmallCinder>();
                int damage = 0;
                float cinderSpeed = Main.rand.NextFloat(2f, 6f);
                if (Main.rand.NextBool(11))
                {
                    type = ModContent.ProjectileType<RancorLargeCinder>();
                    damage = projectile.damage / 3;
                    cinderSpeed *= 1.2f;
                }
                Vector2 cinderVelocity = Vector2.Lerp(-projectile.velocity, -Vector2.UnitY, 0.45f).RotatedByRandom(0.72f) * cinderSpeed;
                Projectile.NewProjectile(endOfLaser, cinderVelocity, type, damage, 0f, projectile.owner);
            }

            FusableParticleManager.GetParticleSetByType<RancorGroundLavaParticleSet>().SpawnParticle(endOfLaser + Main.rand.NextVector2Circular(10f, 10f) + projectile.velocity * 40f, 135f);
        }

        private float PrimitiveWidthFunction(float completionRatio) => projectile.scale * 20f;

        private Color PrimitiveColorFunction(float completionRatio)
        {
            Color vibrantColor = Color.Lerp(Color.Blue, Color.Red, (float)Math.Cos(Main.GlobalTime * 0.67f - completionRatio / LaserLength * 29f) * 0.5f + 0.5f);
            float opacity = projectile.Opacity * Utils.InverseLerp(0.97f, 0.9f, completionRatio, true) *
                Utils.InverseLerp(0f, MathHelper.Clamp(15f / LaserLength, 0f, 0.5f), completionRatio, true) *
                (float)Math.Pow(Utils.InverseLerp(60f, 270f, LaserLength, true), 3D);
            return Color.Lerp(vibrantColor, Color.White, 0.5f) * opacity * 2f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (RayDrawer is null)
                RayDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:Flame"]);

            GameShaders.Misc["CalamityMod:Flame"].UseImage("Images/Misc/Perlin");

            Vector2[] basePoints = new Vector2[24];
            for (int i = 0; i < basePoints.Length; i++)
                basePoints[i] = projectile.Center + projectile.velocity * i / (basePoints.Length - 1f) * LaserLength;

            Vector2 overallOffset = -Main.screenPosition;
            RayDrawer.Draw(basePoints, overallOffset, 92);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsOverWiresUI.Add(index);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<RancorBurn>(), 150);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}

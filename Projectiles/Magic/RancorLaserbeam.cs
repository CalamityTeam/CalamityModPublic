using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Graphics.Primitives;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorLaserbeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public Projectile MagicCircle
        {
            get
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].identity != Projectile.ai[0] || !Main.projectile[i].active || Main.projectile[i].owner != Projectile.owner)
                        continue;

                    return Main.projectile[i];
                }
                return null;
            }
        }
        public ref float LaserLength => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float MaxLaserLength = 3330f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            // If the owner is no longer able to cast the beam, kill it.
            if (!Owner.channel || Owner.noItems || Owner.CCed || MagicCircle is null)
            {
                Projectile.Kill();
                return;
            }

            // Grow bigger up to a point.
            Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.15f, 0.05f, 2f);

            // Decide where to position the laserbeam.
            Vector2 circlePointDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);
            Projectile.Center = MagicCircle.Center;

            // Update the laser length.
            float[] laserLengthSamplePoints = new float[24];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.scale * 8f, MaxLaserLength, laserLengthSamplePoints);
            LaserLength = laserLengthSamplePoints.Average();

            // Update aim.
            UpdateAim();

            // Adjust damage every frame. This is necessary to ensure that mana sickness and such are applied.
            Projectile.damage = (int)Owner.GetTotalDamage<MagicDamageClass>().ApplyTo(MagicCircle.damage);

            // Create arms on surfaces.
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(8))
                CreateArmsOnSurfaces();

            // Create hit effects at the end of the beam.
            if (Main.myPlayer == Projectile.owner)
                CreateTileHitEffects();

            // Make the beam cast light along its length. The brightness of the light is reliant on the scale of the beam.
            DelegateMethods.v3_1 = Color.DarkViolet.ToVector3() * Projectile.scale * 0.4f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public void UpdateAim()
        {
            // Only execute the aiming code for the owner.
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 newAimDirection = MagicCircle.velocity.SafeNormalize(Vector2.UnitY);

            // Sync if the direction is different from the old one.
            // Spam caps are ignored due to the frequency of this happening.
            if (newAimDirection != Projectile.velocity)
            {
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            Projectile.velocity = newAimDirection;
        }

        public void CreateArmsOnSurfaces()
        {
            Vector2 endOfLaser = Projectile.Center + Projectile.velocity * LaserLength + Main.rand.NextVector2Circular(80f, 8f);
            Vector2 idealCenter = endOfLaser;
            if (WorldUtils.Find(idealCenter.ToTileCoordinates(), Searches.Chain(new Searches.Down(5), new CustomConditions.SolidOrPlatform()), out Point result))
            {
                idealCenter = result.ToWorldCoordinates();
            }
            Point endOfLaserTileCoords = idealCenter.ToTileCoordinates();
            Tile endTile = CalamityUtils.ParanoidTileRetrieval(endOfLaserTileCoords.X, endOfLaserTileCoords.Y);

            if (endTile.HasUnactuatedTile && (Main.tileSolid[endTile.TileType] || Main.tileSolidTop[endTile.TileType]) && !endTile.IsHalfBlock && endTile.Slope == 0)
            {
                Vector2 armSpawnPosition = endOfLaserTileCoords.ToWorldCoordinates();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), armSpawnPosition, Vector2.Zero, ModContent.ProjectileType<RancorArm>(), Projectile.damage * 2 / 3, 0f, Projectile.owner);
            }
        }

        public void CreateTileHitEffects()
        {
            Vector2 endOfLaser = Projectile.Center + Projectile.velocity * (LaserLength - Main.rand.NextFloat(12f, 72f));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), endOfLaser, Main.rand.NextVector2Circular(4f, 8f), ModContent.ProjectileType<RancorFog>(), 0, 0f, Projectile.owner);

            if (Main.rand.NextBool())
            {
                int type = ModContent.ProjectileType<RancorSmallCinder>();
                int damage = 0;
                float cinderSpeed = Main.rand.NextFloat(2f, 6f);
                if (Main.rand.NextBool(11))
                {
                    type = ModContent.ProjectileType<RancorLargeCinder>();
                    damage = Projectile.damage / 3;
                    cinderSpeed *= 1.2f;
                }
                Vector2 cinderVelocity = Vector2.Lerp(-Projectile.velocity, -Vector2.UnitY, 0.45f).RotatedByRandom(0.72f) * cinderSpeed;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), endOfLaser, cinderVelocity, type, damage, 0f, Projectile.owner);
            }

            RancorLavaMetaball.SpawnParticle(endOfLaser + Main.rand.NextVector2Circular(10f, 10f) + Projectile.velocity * 40f, 135f);
        }

        private float PrimitiveWidthFunction(float completionRatio) => Projectile.scale * 20f;

        private Color PrimitiveColorFunction(float completionRatio)
        {
            Color vibrantColor = Color.Lerp(Color.Blue, Color.Red, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.67f - completionRatio / LaserLength * 29f) * 0.5f + 0.5f);
            float opacity = Projectile.Opacity * Utils.GetLerpValue(0.97f, 0.9f, completionRatio, true) *
                Utils.GetLerpValue(0f, MathHelper.Clamp(15f / LaserLength, 0f, 0.5f), completionRatio, true) *
                (float)Math.Pow(Utils.GetLerpValue(60f, 270f, LaserLength, true), 3D);
            return Color.Lerp(vibrantColor, Color.White, 0.5f) * opacity * 2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:Flame"].UseImage1("Images/Misc/Perlin");

            Vector2[] basePoints = new Vector2[24];
            for (int i = 0; i < basePoints.Length; i++)
                basePoints[i] = Projectile.Center + Projectile.velocity * i / (basePoints.Length - 1f) * LaserLength;

            PrimitiveSet.Prepare(basePoints, new(PrimitiveWidthFunction, PrimitiveColorFunction, shader: GameShaders.Misc["CalamityMod:Flame"]), 92);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<RancorBurn>(), 150);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}

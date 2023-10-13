using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PristineFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Particles/MediumMist";

        // Helix trail thing
        public PrimitiveTrail HelixTrail = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.25f);

            // Light smoke
            Color smokeColor = new Color(255, Main.rand.Next(160, 230 + 1), 100, 100);
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 16, Main.rand.NextFloat(0.6f, 1.2f), 0.2f, glowing: true);
            GeneralParticleHandler.SpawnParticle(smoke);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(50);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            
            // Big poofy column
            for (int i = 0; i < 10; i++)
            {
                Vector2 smokePos = Projectile.Center + Main.rand.NextVector2Circular(32f, 32f);
                Vector2 smokeVel = Vector2.UnitY * Main.rand.NextFloat(-24f, -4f);
                Particle smoke = new MediumMistParticle(smokePos, smokeVel, new Color(255, 220, 100), Color.Black, Main.rand.NextFloat(0.6f, 1.6f), 225 - Main.rand.Next(60), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public float HelixTrailWidthFunction(float completionRatio) => Utils.GetLerpValue(1f, 0.4f, completionRatio, true) * 2.5f;
        public Color HelixTrailColorFunction(float completionRatio) => new Color(255, 220, 100) * (1f - completionRatio);

        public override bool PreDraw(ref Color lightColor)
        {
            if (HelixTrail is null)
                HelixTrail = new PrimitiveTrail(HelixTrailWidthFunction, HelixTrailColorFunction);

            // Draw a double helix trail
            for (int direction = -1; direction <= 1; direction += 2)
            {
                List<Vector2> trailPositions = new List<Vector2>();
                int trailPoints = ProjectileID.Sets.TrailCacheLength[Type];
                // Add points adjusted for sine
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                        break;

                    Vector2 sinOffset = (Vector2.UnitY * direction * MathF.Sin(i * MathHelper.Pi * 0.125f) * 24f).RotatedBy(Projectile.oldRot[i]);
                    trailPositions.Add(Projectile.oldPos[i] + sinOffset);
                }
                HelixTrail.Draw(trailPositions, Projectile.Size * 0.5f- Main.screenPosition, 60);
            }
            return false;
        }
    }
}

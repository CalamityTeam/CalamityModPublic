using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class RadiantResolutionAura : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public ref float AllocatedSlots => ref Projectile.ai[0];
        public ref float GeneralTimer => ref Projectile.ai[1];

        public const float TargetCheckDistance = 1600f;
        public const int RadiantOrbAppearRateLowerBound = 7;
        public const int RadiantOrbDamageUpperBound = 10000;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Aura");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void AI()
        {
            // Emit some light.
            Lighting.AddLight(Projectile.Center, Vector3.One * 1.2f);

            // Ensure that the projectile using this AI is the correct projectile and that the owner has the appropriate buffs.
            VerifyIdentityOfCaller();

            // Store the allocated slots in the minionSlots field so that the amount of slots the projectile is holding
            // is always correct.
            Projectile.minionSlots = Projectile.ai[0];

            // Stay near the target and spin around.
            Projectile.Center = Owner.Center - Vector2.UnitY * 16f;
            Projectile.rotation += MathHelper.ToRadians(AllocatedSlots * 0.85f + 3f);

            float damageMultiplier = (float)Math.Log(AllocatedSlots, 3D) + 1f;

            // Softcap the multiplier after it has exceeded 3x the base value.
            float softcappedDamageMultiplier = damageMultiplier;
            if (softcappedDamageMultiplier > 3f)
                softcappedDamageMultiplier = ((damageMultiplier - 3f) * 0.1f) + 3f;

            int radiantOrbDamage = (int)(Projectile.damage * softcappedDamageMultiplier);
            int radiantOrbAppearRate = (int)(130 * Math.Pow(0.9, AllocatedSlots));

            // Hard-cap the orb appear rate and damage.
            // The latter is basically impossible to reach now due to rebalancing, but it shall remain for the time being.
            if (radiantOrbAppearRate < RadiantOrbAppearRateLowerBound)
                radiantOrbAppearRate = RadiantOrbAppearRateLowerBound;

            if (radiantOrbDamage > RadiantOrbDamageUpperBound)
                radiantOrbDamage = RadiantOrbDamageUpperBound;

            // Attack nearby targets.
            GeneralTimer++;
            NPC potentialTarget = Projectile.Center.MinionHoming(TargetCheckDistance, Owner);
            if (potentialTarget != null && Main.myPlayer == Projectile.owner)
                AttackTarget(potentialTarget, radiantOrbAppearRate, radiantOrbDamage);
        }

        public void VerifyIdentityOfCaller()
        {
            Owner.AddBuff(ModContent.BuffType<SarosPossessionBuff>(), 3600);
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<RadiantResolutionAura>();
            if (isCorrectProjectile)
            {
                if (Owner.dead)
                    Owner.Calamity().radiantResolution = false;

                if (Owner.Calamity().radiantResolution)
                    Projectile.timeLeft = 2;
            }
        }

        public void AttackTarget(NPC target, int radiantOrbAppearRate, int radiantOrbDamage)
        {
            if (GeneralTimer % 35f == 34f)
            {
                for (int i = 0; i < 2; i++)
                {
                    float angle = MathHelper.Lerp(-MathHelper.ToRadians(20f), MathHelper.ToRadians(20f), i / 2f);
                    Vector2 fireVelocity = Projectile.SafeDirectionTo(target.Center).RotatedBy(angle) * 15f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity, ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, Projectile.knockBack, Projectile.owner);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = radiantOrbDamage / 2;
                }
            }

            if (GeneralTimer % radiantOrbAppearRate == radiantOrbAppearRate - 1)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 360f);
                Vector2 bootlegRadianceOrbVelocity = Projectile.SafeDirectionTo(target.Center) * 2f;
                int p2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, bootlegRadianceOrbVelocity, ModContent.ProjectileType<RadiantResolutionOrb>(), radiantOrbDamage, Projectile.knockBack * 4f, Projectile.owner);
                if (Main.projectile.IndexInRange(p2))
                    Main.projectile[p2].originalDamage = radiantOrbDamage;
                for (int i = 0; i < 3; i++)
                {
                    float angle = MathHelper.Lerp(-MathHelper.ToRadians(30f), MathHelper.ToRadians(30f), i / 3f);
                    Vector2 fireVelocity = Projectile.SafeDirectionTo(target.Center).RotatedBy(angle) * 19f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity, ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, Projectile.knockBack, Projectile.owner);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = radiantOrbDamage / 2;
                }
            }
        }

        public override bool? CanDamage() => false;

        public override void PostDraw(Color lightColor)
        {
            Texture2D currentTexture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(currentTexture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation + MathHelper.PiOver2,
                currentTexture.Size() / 2f,
                1f,
                SpriteEffects.None,
                0);
        }
    }
}

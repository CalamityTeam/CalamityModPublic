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
        public Player Owner => Main.player[projectile.owner];
        public ref float AllocatedSlots => ref projectile.ai[0];
        public ref float GeneralTimer => ref projectile.ai[1];

        public const float TargetCheckDistance = 1600f;
        public const int RadiantOrbAppearRateLowerBound = 7;
        public const int RadiantOrbDamageUpperBound = 10000;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Aura");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 66;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void AI()
        {
            // Emit some light.
            Lighting.AddLight(projectile.Center, Vector3.One * 1.2f);

            // Ensure that the projectile using this AI is the correct projectile and that the owner has the appropriate buffs.
            VerifyIdentityOfCaller();

            // Handle dynamic minion damage.
            HandleDynamicMinionDamage();

            // Store the allocated slots in the minionSlots field so that the amount of slots the projectile is holding
            // is always correct.
            projectile.minionSlots = projectile.ai[0];

            // Stay near the target and spin around.
            projectile.Center = Owner.Center - Vector2.UnitY * 16f;
            projectile.rotation += MathHelper.ToRadians(AllocatedSlots * 0.85f + 3f);

            float damageMultiplier = (float)Math.Log(AllocatedSlots, 3D) + 1f;

            // Softcap the multiplier after it has exceeded 3x the base value.
            float softcappedDamageMultiplier = damageMultiplier;
            if (softcappedDamageMultiplier > 3f)
                softcappedDamageMultiplier = ((damageMultiplier - 3f) * 0.1f) + 3f;

            int radiantOrbDamage = (int)(projectile.damage * softcappedDamageMultiplier);
            int radiantOrbAppearRate = (int)(130 * Math.Pow(0.9, AllocatedSlots));

            // Hard-cap the orb appear rate and damage.
            // The latter is basically impossible to reach now due to rebalancing, but it shall remain for the time being.
            if (radiantOrbAppearRate < RadiantOrbAppearRateLowerBound)
                radiantOrbAppearRate = RadiantOrbAppearRateLowerBound;

            if (radiantOrbDamage > RadiantOrbDamageUpperBound)
                radiantOrbDamage = RadiantOrbDamageUpperBound;

            // Attack nearby targets.
            GeneralTimer++;
            NPC potentialTarget = projectile.Center.MinionHoming(TargetCheckDistance, Owner);
            if (potentialTarget != null && Main.myPlayer == projectile.owner)
                AttackTarget(potentialTarget, radiantOrbAppearRate, radiantOrbDamage);
        }

        public void VerifyIdentityOfCaller()
        {
            Owner.AddBuff(ModContent.BuffType<RadiantResolutionBuff>(), 3600);
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<RadiantResolutionAura>();
            if (isCorrectProjectile)
            {
                if (Owner.dead)
                    Owner.Calamity().radiantResolution = false;

                if (Owner.Calamity().radiantResolution)
                    projectile.timeLeft = 2;
            }
        }

        public void HandleDynamicMinionDamage()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Owner.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public void AttackTarget(NPC target, int radiantOrbAppearRate, int radiantOrbDamage)
        {
            if (GeneralTimer % 35f == 34f)
            {
                for (int i = 0; i < 2; i++)
                {
                    float angle = MathHelper.Lerp(-MathHelper.ToRadians(20f), MathHelper.ToRadians(20f), i / 2f);
                    Vector2 fireVelocity = projectile.SafeDirectionTo(target.Center).RotatedBy(angle) * 15f;
                    Projectile.NewProjectile(projectile.Center, fireVelocity, ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, projectile.knockBack, projectile.owner);
                }
            }

            if (GeneralTimer % radiantOrbAppearRate == radiantOrbAppearRate - 1)
            {
                Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 360f);
                Vector2 bootlegRadianceOrbVelocity = projectile.SafeDirectionTo(target.Center) * 2f;
                Projectile.NewProjectile(spawnPosition, bootlegRadianceOrbVelocity, ModContent.ProjectileType<RadiantResolutionOrb>(), radiantOrbDamage, projectile.knockBack * 4f, projectile.owner);
                for (int i = 0; i < 3; i++)
                {
                    float angle = MathHelper.Lerp(-MathHelper.ToRadians(30f), MathHelper.ToRadians(30f), i / 3f);
                    Vector2 fireVelocity = projectile.SafeDirectionTo(target.Center).RotatedBy(angle) * 19f;
                    Projectile.NewProjectile(projectile.Center, fireVelocity, ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, projectile.knockBack, projectile.owner);
                }
            }
        }

        public override bool CanDamage() => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D currentTexture = ModContent.GetTexture(Texture);
            spriteBatch.Draw(currentTexture,
                projectile.Center - Main.screenPosition,
                null,
                lightColor,
                projectile.rotation + MathHelper.PiOver2,
                currentTexture.Size() / 2f,
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}

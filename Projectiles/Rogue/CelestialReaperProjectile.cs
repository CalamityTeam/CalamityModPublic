using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CelestialReaperProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CelestialReaper";

        public int HomingCooldown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Reaper");
        }

        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 76;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 6;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            projectile.rotation += MathHelper.ToRadians(30f) / (float)Math.Log(6f - projectile.penetrate + 2f) / 1.4f; // Slow down the more hits the scythe has accumulated.
            if (HomingCooldown > 0)
            {
                HomingCooldown--;
            }
            else
            {
                NPC target = projectile.Center.ClosestNPCAt(640f);
                if (target != null)
                    projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(target.Center) * 20f) / 21f;
            }

            // This code is only run on stealth strikes and periodically spawns damaging afterimages.
            if (projectile.ai[0] == 1f)
            {
                // Afterimages are spawned three times as fast after at least one hit has occurred.
                float framesNeeded = projectile.numHits > 0 ? 20f : 60f;
                if (projectile.timeLeft % (int)framesNeeded == 0f)
                {
                    int projID = ModContent.ProjectileType<CelestialReaperAfterimage>();
                    int damage = (int)(projectile.damage * 0.25f);
                    float kb = projectile.knockBack * 0.5f;
                    Projectile.NewProjectile(projectile.Center, projectile.velocity, projID, damage, kb, projectile.owner);
                }
            }
        }

        // The explicit (bool?) cast is necessary until C# 9.0. How ugly.
        public override bool? CanHitNPC(NPC target) => HomingCooldown > 0 ? false : (bool?)null;

        public override bool CanHitPvp(Player target) => HomingCooldown <= 0;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HomingCooldown = 25;
            projectile.velocity *= -0.75f; // Bounce off of the enemy.
        }

        public override void Kill(int timeLeft)
        {
            bool ss = projectile.Calamity().stealthStrike;
            int numSplits = 4;
            int projID = ModContent.ProjectileType<CelestialReaperAfterimage>();
            int damage = (int)(projectile.damage * (ss ? 0.25f : 0.5f));
            float kb = projectile.knockBack * 0.5f;
            float speed = 12f;
            for (float i = 0; i < numSplits; ++i)
            {
                float angle = MathHelper.TwoPi * i / numSplits;
                Vector2 velocity = angle.ToRotationVector2() * speed;
                Projectile.NewProjectile(projectile.Center, velocity, projID, damage, kb, projectile.owner);
            }
        }
    }
}

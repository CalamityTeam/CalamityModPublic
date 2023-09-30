using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CelestialReaperProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CelestialReaper";

        public int HomingCooldown = 0;

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 76;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(30f) / (float)Math.Log(6f - Projectile.penetrate + 2f) / 1.4f; // Slow down the more hits the scythe has accumulated.
            if (HomingCooldown > 0)
            {
                HomingCooldown--;
            }
            else
            {
                NPC target = Projectile.Center.ClosestNPCAt(640f);
                if (target != null)
                    Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(target.Center) * 20f) / 21f;
            }

            // This code is only run on stealth strikes and periodically spawns damaging afterimages.
            if (Projectile.ai[0] == 1f)
            {
                // Afterimages are spawned three times as fast after at least one hit has occurred.
                float framesNeeded = Projectile.numHits > 0 ? 20f : 60f;
                if (Projectile.timeLeft % (int)framesNeeded == 0f)
                {
                    int projID = ModContent.ProjectileType<CelestialReaperAfterimage>();
                    int damage = (int)(Projectile.damage * 0.25f);
                    float kb = Projectile.knockBack * 0.5f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, projID, damage, kb, Projectile.owner);
                }
            }
        }

        // The explicit (bool?) cast is necessary until C# 9.0. How ugly.
        public override bool? CanHitNPC(NPC target) => HomingCooldown > 0 ? false : (bool?)null;

        public override bool CanHitPvp(Player target) => HomingCooldown <= 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HomingCooldown = 25;
            Projectile.velocity *= -0.75f; // Bounce off of the enemy.
        }

        public override void OnKill(int timeLeft)
        {
            bool ss = Projectile.Calamity().stealthStrike;
            int numSplits = 4;
            int projID = ModContent.ProjectileType<CelestialReaperAfterimage>();
            int damage = (int)(Projectile.damage * (ss ? 0.25f : 0.5f));
            float kb = Projectile.knockBack * 0.5f;
            float speed = 12f;
            for (float i = 0; i < numSplits; ++i)
            {
                float angle = MathHelper.TwoPi * i / numSplits;
                Vector2 velocity = angle.ToRotationVector2() * speed;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projID, damage, kb, Projectile.owner);
            }
        }
    }
}

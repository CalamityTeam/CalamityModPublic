using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BlindingLight : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float Radius = 1400f;
        private const int Lifetime = 45;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blinding Light");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = Lifetime;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = projectile.Distance(targetHitbox.TopLeft());
            float dist2 = projectile.Distance(targetHitbox.TopRight());
            float dist3 = projectile.Distance(targetHitbox.BottomLeft());
            float dist4 = projectile.Distance(targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= Radius;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => crit = true;

        public override void AI()
        {
            if (projectile.timeLeft == Lifetime)
            {
                ConsumeNearbyBlades();
                DivideDamageAmongstTargets();
            }

            projectile.ai[0]++;
            float progress = (float)Math.Sin(projectile.ai[0] / Lifetime * MathHelper.Pi);
            if (projectile.ai[0] > 55f)
                progress = MathHelper.Lerp(progress, 0f, (projectile.ai[0] - 55f) / 5f);
            if (Main.netMode != NetmodeID.Server && projectile.ai[0] > 15f) // Otherwise a white flash appears, but it quickly disappears.
            {
                if (!Filters.Scene["CalamityMod:LightBurst"].IsActive())
                    Filters.Scene.Activate("CalamityMod:LightBurst", projectile.Center).GetShader().UseTargetPosition(projectile.Center).UseProgress(0f);

                Filters.Scene["CalamityMod:LightBurst"].GetShader().UseProgress(progress);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
                Filters.Scene.Deactivate("CalamityMod:LightBurst");
        }

        private void ConsumeNearbyBlades()
        {
            int lightBlade = ModContent.ProjectileType<LightBlade>();
            int extraDamage = 0;
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile otherProj = Main.projectile[i];
                if (otherProj is null || !otherProj.active || otherProj.owner != projectile.owner || otherProj.type != lightBlade)
                    continue;

                // Can only consume blades within the flash radius (which should be most if not all of them anyway)
                if (projectile.Distance(otherProj.Center) > Radius)
                    continue;
                extraDamage += otherProj.damage / 2;
                otherProj.Kill();
            }
            projectile.damage += extraDamage;
        }

        private void DivideDamageAmongstTargets()
        {
            int numTargets = 0;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (npc is null || !npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                    continue;
                if (projectile.Colliding(default, npc.Hitbox))
                    ++numTargets;
            }

            // The number of targets is minimum one to prevent dividing by zero.
            if (numTargets <= 0)
                numTargets = 1;

            // Divide damage by the square root of nearby targets. 25 targets = 1/5th damage, for example.
            projectile.damage = (int)(projectile.damage / Math.Sqrt(numTargets));
        }
    }
}

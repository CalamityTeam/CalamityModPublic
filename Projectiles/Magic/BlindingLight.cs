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
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

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
                ConsumeNearbyBlades();

            projectile.ai[0]++;
            float progress = (float)Math.Sin(projectile.ai[0] / Lifetime * MathHelper.Pi);
            if (projectile.ai[0] > 55f)
                progress = MathHelper.Lerp(progress, 0f, (projectile.ai[0] - 55f) / 5f);
            if (projectile.ai[0] > 15f) // Otherwise a white flash appears, but it quickly disappears.
            {
                if (Main.netMode != NetmodeID.Server && !Filters.Scene["CalamityMod:LightBurst"].IsActive())
                {
                    Filters.Scene.Activate("CalamityMod:LightBurst", projectile.Center).GetShader().UseTargetPosition(projectile.Center).UseProgress(0f);
                }
                Filters.Scene["CalamityMod:LightBurst"].GetShader().UseProgress(progress);
            }
        }
        public override void Kill(int timeLeft)
        {
            Filters.Scene.Deactivate("CalamityMod:LightBurst");
        }
        private void ConsumeNearbyBlades()
        {
            int lightBlade = ModContent.ProjectileType<LightBlade>();
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile otherProj = Main.projectile[i];
                if (otherProj is null || !otherProj.active || otherProj.owner != projectile.owner || otherProj.type != lightBlade)
                    continue;

                projectile.damage += otherProj.damage;
                otherProj.Kill();
            } 
        }
    }
}

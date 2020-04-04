using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BlindingLight : ModProjectile
    {
        private static readonly FieldInfo reqLight = typeof(MoonlordDeathDrama).GetField("requestedLight", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly FieldInfo actualLight = typeof(MoonlordDeathDrama).GetField("whitening", BindingFlags.NonPublic | BindingFlags.Static);
        private const float Radius = 1400f;
        private const int Lifetime = 16;
        
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
            
            MoonlordDeathDrama.RequestLight(1f, projectile.Center);
            float currentLight = (float)reqLight.GetValue(null);
            if (currentLight > 0f)
            {
                float newLight = MathHelper.Clamp(currentLight + 0.12f, 0f, 1f);
                reqLight.SetValue(null, newLight);
                actualLight.SetValue(null, newLight);
            }
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

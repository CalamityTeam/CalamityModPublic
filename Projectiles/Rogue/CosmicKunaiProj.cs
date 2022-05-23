using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CosmicKunaiProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CosmicKunai";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kunai");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            Projectile.alpha += 17;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.5f / 255f);
            CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 300f, 12f, 20f);
        }
    }
}

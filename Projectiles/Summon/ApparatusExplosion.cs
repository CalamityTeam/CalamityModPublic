﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class ApparatusExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Bluish cyan light
            Lighting.AddLight(Projectile.Center, 0.1f, 0.5f, 1f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
            }
        }
    }
}

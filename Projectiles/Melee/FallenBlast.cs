using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Projectiles.Melee
{
    public class FallenBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults() => Main.projFrames[Type] = 8;
        private static float ExplosionRadius = 236f;

        public override void SetDefaults()
        {
            Projectile.width = 472;
            Projectile.height = 472;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 600;
            Projectile.timeLeft = 24;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            if (Projectile.frame >= 8)
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);

    }
}


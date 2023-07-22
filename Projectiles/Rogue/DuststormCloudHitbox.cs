using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Utilities;
using CalamityMod.Items.Weapons.Rogue;

namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormCloudHitbox : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = DuststormInABottle.CloudLifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.scale < (Projectile.ai[1]==1 ? DuststormInABottle.MaxSizeStealth : DuststormInABottle.MaxSize))
            {
                Projectile.scale += (Projectile.ai[1] == 1 ? DuststormInABottle.StealthGrowhRate : DuststormInABottle.GrowthRate);
                Projectile.ExpandHitboxBy(1+ (Projectile.ai[1] == 1 ? DuststormInABottle.StealthGrowhRate : DuststormInABottle.GrowthRate));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width, targetHitbox);
    }
}

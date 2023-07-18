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
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.scale < (Projectile.Calamity().stealthStrike ? DuststormInABottle.MaxSizeStealth : DuststormInABottle.MaxSize)) // 3.5 or 3, it scales exponentially
            {
                Projectile.scale += DuststormInABottle.GrowthRate;
                Projectile.ExpandHitboxBy(1+DuststormInABottle.GrowthRate);
            }
            if (Main.rand.NextBool(4))
            {
                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 85, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index].noGravity = true;
                Main.dust[index].fadeIn = 1.5f;
                Main.dust[index].velocity *= 0.25f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width, targetHitbox);
    }
}

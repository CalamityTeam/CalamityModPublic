using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YoungDukeToothballSpike : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float TimerForHitbox => ref Projectile.ai[0];

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 26;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (TimerForHitbox < 30f)
                TimerForHitbox++;
        }

        public override bool? CanDamage() => TimerForHitbox >= 30f;
    }
}

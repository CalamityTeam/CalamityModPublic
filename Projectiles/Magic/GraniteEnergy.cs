using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GraniteEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 90;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 60 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.ToRadians(90);
            if (Main.rand.NextBool())
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 0.6f);

            if (Projectile.timeLeft < 60)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 12f, 20f);
        }
    }
}

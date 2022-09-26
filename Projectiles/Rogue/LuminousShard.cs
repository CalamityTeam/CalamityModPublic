using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LuminousShard : ModProjectile
    {
        bool gravity = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust Shard");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            if (Projectile.ai[0] == 0f && Projectile.velocity.X == 0f && Projectile.velocity.Y == -2f)
                gravity = true;

            Projectile.ai[0] = 1f;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            if (gravity)
                Projectile.velocity.Y *= 1.05f;

            if (Projectile.timeLeft < 90)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 10f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X / 4, Projectile.oldVelocity.Y / 4, 0, default, 0.75f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 177, Projectile.oldVelocity.X / 4, Projectile.oldVelocity.Y / 4, 0, default, 0.75f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class HydrothermicSphere : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Sphere");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 170 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 100, default, 2f);
            fire.noGravity = true;
            fire.velocity = Vector2.Zero;

            if (Projectile.timeLeft < 170)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 600f, 9f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AtaxiaOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 200;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 170 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            Dust fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
            fire.noGravity = true;
            fire.velocity = Vector2.Zero;

            if (projectile.timeLeft < 170)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 9f, 20f);
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

using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Healing
{
    public class DemonHeal : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Heal");

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 640;
        }

        public override void AI()
        {
            projectile.HealingProjectile(10, projectile.owner, 20f, 20f, true, 640);

            Dust fire = Dust.NewDustPerfect(projectile.Center, 130);
            fire.velocity = Microsoft.Xna.Framework.Vector2.Zero;
            fire.scale = Main.rand.NextFloat(1f, 1.15f);
            fire.fadeIn = 0.45f;
            fire.noGravity = true;
        }
    }
}

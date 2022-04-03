using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Healing
{
    public class DemonHeal : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Heal");

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 640;
        }

        public override void AI()
        {
            Projectile.HealingProjectile(10, Projectile.owner, 20f, 20f, true, 640);

            Dust fire = Dust.NewDustPerfect(Projectile.Center, 130);
            fire.velocity = Microsoft.Xna.Framework.Vector2.Zero;
            fire.scale = Main.rand.NextFloat(1f, 1.15f);
            fire.fadeIn = 0.45f;
            fire.noGravity = true;
        }
    }
}

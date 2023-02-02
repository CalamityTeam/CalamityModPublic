using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SlimeStream : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silme Stream");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 90;
            Projectile.MaxUpdates = 3;
        }

        public override void AI()
        {
            Dust.QuickDust(Projectile.Center, Projectile.ai[0] > 0f ? Color.Magenta : Color.RoyalBlue);

            if (Projectile.ai[1] > 0f && Projectile.timeLeft < 60)
                CalamityUtils.HomeInOnNPC(Projectile, false, 320f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Slimed, 600);
    }
}

using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BrimstoneBeam5 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Vector2 vector33 = projectile.position;
            vector33 -= projectile.velocity;
            projectile.alpha = 255;
            int num249 = 235;
            int num448 = Dust.NewDust(vector33, 1, 1, num249, 0f, 0f, 0, default, 1.5f);
            Main.dust[num448].position = vector33;
            Main.dust[num448].velocity *= 0.1f;
            Main.dust[num448].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}

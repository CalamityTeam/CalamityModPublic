using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Shaderain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/ShaderainHostile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 3;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.alpha = 50;
        }

        public override void Kill(int timeLeft)
        {
            int num310 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + (float)projectile.height - 2f), 2, 2, 14, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num310];
            dust.position.X -= 2f;
            dust.alpha = 38;
            dust.velocity *= 0.1f;
            dust.velocity += -projectile.oldVelocity * 0.25f;
            dust.scale = 0.95f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 60);
        }
    }
}

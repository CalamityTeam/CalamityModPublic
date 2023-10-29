using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.rotation += 0.12f * Projectile.direction;

            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);

            for (int i = 0; i < 2; i++)
            {
                Vector2 dspeed = -Projectile.velocity * 0.7f;
                int brimDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.1f);
                Main.dust[brimDust].noGravity = true;
                Main.dust[brimDust].velocity = dspeed;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
        }
    }
}

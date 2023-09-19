using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ProfanedSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.45f, 0.35f, 0f);

            if (Projectile.timeLeft < 510)
                Projectile.tileCollide = true;

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver4;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] <= 20f)
                Projectile.velocity *= 0.95f;
            else
                Projectile.velocity *= 1.06f;

            if (Projectile.ai[0] > 40f)
                Projectile.ai[0] = 0f;

            float maxVelocity = 20f;
            if (Projectile.velocity.Length() > maxVelocity)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= maxVelocity;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 150, 0, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }
    }
}

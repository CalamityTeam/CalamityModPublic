using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class FlakAcid : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0]++ <= 30f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.ai[0] / 30f);
            }
            if (Projectile.velocity.Y < 10f)
                Projectile.velocity.Y += 0.15f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].Distance(Projectile.Center) < 60f)
                {
                    Projectile.Kill();
                }
            }
            if (Math.Sign(Projectile.velocity.Y) != Math.Sign(Projectile.oldVelocity.Y) && Projectile.ai[0] >= 5f)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(150);
            Projectile.Damage();
            for (int i = 0; i <= 40; i++)
            {
                int idx = Dust.NewDust(Projectile.position, 200, 200, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (Main.dust[idx].position - Projectile.Center).Length() / 30f;
                Main.dust[idx].scale = 2.5f;
            }
            for (int i = 0; i <= 90; i++)
            {
                int idx = Dust.NewDust(Projectile.Center, 0, 0, (int)CalamityDusts.SulfurousSeaAcid);
                Main.dust[idx].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 8f;
                Main.dust[idx].scale = 3f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividLaser2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            Projectile.ai[1] += 1f;
            for (int dust = 0; dust < 2; dust++)
            {
                Vector2 dustPosOffset = Vector2.UnitX * -12f;
                dustPosOffset = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[1] * 0.1308997f + (float)dust * 3.14159274f), default) * value7 - Projectile.rotation.ToRotationVector2() * 10f;
                int exo = Dust.NewDust(Projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[exo].scale = 0.75f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].position = Projectile.Center + dustPosOffset;
                Main.dust[exo].velocity = Projectile.velocity;
            }

            if (Projectile.timeLeft < 110)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] >= 1f)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 400f, 12f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    107,
                    234,
                    269
                });
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, randomDust, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft >= 110)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.timeLeft < 110;
    }
}

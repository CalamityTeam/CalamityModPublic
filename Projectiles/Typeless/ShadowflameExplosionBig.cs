using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class ShadowflameExplosionBig : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 3f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 Dspeed = new Vector2(4.3f, 4.3f).RotatedBy(MathHelper.ToRadians(Projectile.ai[0]));
                    float Dscale = Main.rand.NextFloat(1f, 1.5f);
                    Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                    Vector2 Ds2 = Dspeed.RotatedBy(MathHelper.ToRadians(120));
                    Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, Ds2.X, Ds2.Y, 0, default, Dscale);
                    Vector2 Ds3 = Dspeed.RotatedBy(MathHelper.ToRadians(240));
                    Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, Ds3.X, Ds3.Y, 0, default, Dscale);
                    Projectile.ai[0] += 19f;
                }
                Projectile.ai[1] = 0f;
            }
            if (Projectile.timeLeft < 25)
            {
                Projectile.damage = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
        }
    }
}

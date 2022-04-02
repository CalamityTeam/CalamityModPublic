using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class ShadowflameExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 75;
            projectile.height = 75;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[1] >= 3f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 Dspeed = new Vector2(2.3f, 2.3f).RotatedBy(MathHelper.ToRadians(projectile.ai[0]));
                    float Dscale = Main.rand.NextFloat(1f, 1.3f);
                    Dust.NewDust(projectile.Center, 1, 1, DustID.Shadowflame, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                    Dust.NewDust(projectile.Center, 1, 1, DustID.Shadowflame, -Dspeed.X, -Dspeed.Y, 0, default, Dscale);
                    projectile.ai[0] += 19f;
                }
                projectile.ai[1] = 0f;
            }
            if (projectile.timeLeft < 25)
            {
                projectile.damage = 0;
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

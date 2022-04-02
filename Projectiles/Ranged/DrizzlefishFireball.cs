using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DrizzlefishFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drizzlefish Fireball");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.aiStyle = 14;
            projectile.timeLeft = 300;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0f);
            int dustType = 235;
            if (projectile.ai[1] == 1f)
            {
                if (Main.rand.NextBool(2))
                {
                    dustType = 35;
                }
                else
                {
                    dustType = 55;
                }
            }
            else
            {
                dustType = 235;
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 5; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= 0.98f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[1] == 1f)
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            }
        }
    }
}

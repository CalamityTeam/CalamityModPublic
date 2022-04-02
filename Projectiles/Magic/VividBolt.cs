using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 100;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.timeLeft = 30;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] == 48f)
            {
                projectile.ai[0] = 0f;
            }
            else
            {
                for (int num41 = 0; num41 < 2; num41++)
                {
                    Vector2 value8 = Vector2.UnitX * -12f;
                    value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 - projectile.rotation.ToRotationVector2() * 10f;
                    int num42 = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[num42].scale = 0.33f;
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = projectile.Center + value8;
                    Main.dust[num42].velocity = projectile.velocity;
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = projectile.position;
                    vector33 -= projectile.velocity * ((float)num447 * 0.25f);
                    int num448 = Dust.NewDust(vector33, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.01f;
                    Main.dust[num448].velocity *= 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }
    }
}

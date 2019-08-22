using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ExoFire : ModProjectile
    {
        public bool speedXChoice = false;
        public bool speedYChoice = false;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fire");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            float speedX = 1f;
            float speedY = 1f;
            if (!speedXChoice)
            {
                speedX = (Main.rand.Next(2) == 0 ? 1.03f : 0.97f);
                speedXChoice = true;
            }
            if (!speedYChoice)
            {
                speedY = (Main.rand.Next(2) == 0 ? 1.03f : 0.97f);
                speedYChoice = true;
            }
            projectile.velocity.X *= speedX;
            projectile.velocity.X *= speedY;
			if (projectile.ai[0] > 7f)
			{
				float num296 = 1f;
				if (projectile.ai[0] == 8f)
				{
					num296 = 0.25f;
				}
				else if (projectile.ai[0] == 9f)
				{
					num296 = 0.5f;
				}
				else if (projectile.ai[0] == 10f)
				{
					num296 = 0.75f;
				}
				projectile.ai[0] += 1f;
				int num297 = (Main.rand.Next(2) == 0 ? 107 : 234);
                if (Main.rand.Next(4) == 0)
                {
                    num297 = 269;
                }
				if (Main.rand.Next(2) == 0)
				{
                    for (int num298 = 0; num298 < 2; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 0.6f);
                        if (Main.rand.Next(3) == 0)
                        {
                            Main.dust[num299].scale *= 1.5f;
                            Dust expr_DBEF_cp_0 = Main.dust[num299];
                            expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 1.2f;
                            Dust expr_DC0F_cp_0 = Main.dust[num299];
                            expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 1.2f;
                        }
                        else
                        {
                            Main.dust[num299].scale *= 0.75f;
                        }
                        Main.dust[num299].noGravity = true;
                        Dust expr_DC74_cp_0 = Main.dust[num299];
                        expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 0.8f;
                        Dust expr_DC94_cp_0 = Main.dust[num299];
                        expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 0.8f;
                        Main.dust[num299].scale *= num296;
                        Main.dust[num299].velocity += projectile.velocity;
                    }
				}
			}
			else
			{
				projectile.ai[0] += 1f;
			}
			projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(mod.BuffType("ExoFreeze"), 30);
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
			target.AddBuff(mod.BuffType("GlacialState"), 120);
			target.AddBuff(mod.BuffType("Plague"), 120);
			target.AddBuff(mod.BuffType("HolyLight"), 120);
			target.AddBuff(BuffID.CursedInferno, 120);
			target.AddBuff(BuffID.Frostburn, 120);
			target.AddBuff(BuffID.OnFire, 120);
			target.AddBuff(BuffID.Ichor, 120);
		}
    }
}

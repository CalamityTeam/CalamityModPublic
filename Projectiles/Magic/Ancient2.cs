using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Ancient2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.extraUpdates = 12;
            projectile.timeLeft = 30;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.3f, 0.25f, 0f);
            if (projectile.timeLeft > 30)
            {
                projectile.timeLeft = 30;
            }
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
                int num297 = 32;
                for (int num298 = 0; num298 < 2; num298++)
                {
                    int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num299].noGravity = true;
                        Main.dust[num299].scale *= 1.5f;
                        Dust expr_DBEF_cp_0 = Main.dust[num299];
                        expr_DBEF_cp_0.velocity.X *= 3f;
                        Dust expr_DC0F_cp_0 = Main.dust[num299];
                        expr_DC0F_cp_0.velocity.Y *= 3f;
                    }
                    Dust expr_DC74_cp_0 = Main.dust[num299];
                    expr_DC74_cp_0.velocity.X *= 2f;
                    Dust expr_DC94_cp_0 = Main.dust[num299];
                    expr_DC94_cp_0.velocity.Y *= 2f;
                    Main.dust[num299].scale *= num296;
                }
                for (int num298 = 0; num298 < 2; num298++)
                {
                    int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[num299].noGravity = true;
                        Main.dust[num299].scale *= 3f;
                        Dust expr_DBEF_cp_0 = Main.dust[num299];
                        expr_DBEF_cp_0.velocity.X *= 2f;
                        Dust expr_DC0F_cp_0 = Main.dust[num299];
                        expr_DC0F_cp_0.velocity.Y *= 2f;
                    }
                    else
                    {
                        Main.dust[num299].scale *= 2f;
                    }
                    Dust expr_DC74_cp_0 = Main.dust[num299];
                    expr_DC74_cp_0.velocity.X *= 1.2f;
                    Dust expr_DC94_cp_0 = Main.dust[num299];
                    expr_DC94_cp_0.velocity.Y *= 1.2f;
                    Main.dust[num299].scale *= num296;
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }
    }
}

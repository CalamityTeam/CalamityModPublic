using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DrizzlefishFireSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/DrizzlefishFire";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drizzlefish Flames");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 90;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 15;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            if (projectile.ai[1] == 1f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Ranged/DrizzlefishFire2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 16, 16)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void AI()
        {
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
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0f);
            if (projectile.timeLeft > 90)
            {
                projectile.timeLeft = 90;
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
                for (int num298 = 0; num298 < 2; num298++)
                {
                    int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[num299].noGravity = true;
                        Main.dust[num299].scale *= 2.5f;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[1] == 1f)
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            }
        }
    }
}

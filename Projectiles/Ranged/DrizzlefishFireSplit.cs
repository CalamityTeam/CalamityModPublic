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
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 90;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Changes the texture of the projectile
            if (Projectile.ai[1] == 1f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DrizzlefishFire2").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 16, 16)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), Projectile.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }

        public override void AI()
        {
            int dustType = 235;
            if (Projectile.ai[1] == 1f)
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
            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);
            if (Projectile.timeLeft > 90)
            {
                Projectile.timeLeft = 90;
            }
            if (Projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                Projectile.ai[0] += 1f;
                for (int num298 = 0; num298 < 2; num298++)
                {
                    int num299 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
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
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[1] == 1f)
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
            else
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 30);
            }
        }
    }
}

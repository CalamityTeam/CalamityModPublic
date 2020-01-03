using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EndoFire : ModProjectile
    {
        public bool speedXChoice = false;
        public bool speedYChoice = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endo Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = -1;
            projectile.extraUpdates = 10;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            float speedX = 1f;
            float speedY = 1f;
            if (!speedXChoice)
            {
                speedX = Main.rand.NextBool(2) ? 1.03f : 0.97f;
                speedXChoice = true;
            }
            if (!speedYChoice)
            {
                speedY = Main.rand.NextBool(2) ? 1.03f : 0.97f;
                speedYChoice = true;
            }
            projectile.velocity.X *= speedX;
            projectile.velocity.X *= speedY;
            if (projectile.ai[0] > 20f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 21f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 22f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 23f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = Main.rand.NextBool(2) ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    num297 = 80;
                }
                if (Main.rand.NextBool(2))
                {

                    int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 0.8f);
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[num299].scale *= 1.5f;
                        Dust expr_DBEF_cp_0 = Main.dust[num299];
                        expr_DBEF_cp_0.velocity.X *= 1.2f;
                        Dust expr_DC0F_cp_0 = Main.dust[num299];
                        expr_DC0F_cp_0.velocity.Y *= 1.2f;
                    }
                    else
                    {
                        Main.dust[num299].scale *= 0.85f;
                    }
                    Main.dust[num299].noGravity = true;
                    Dust expr_DC74_cp_0 = Main.dust[num299];
                    expr_DC74_cp_0.velocity.X *= 0.8f;
                    Dust expr_DC94_cp_0 = Main.dust[num299];
                    expr_DC94_cp_0.velocity.Y *= 0.8f;
                    Main.dust[num299].scale *= num296;
                    Main.dust[num299].velocity += projectile.velocity;
                    Main.dust[num299].noLight = true;

                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 90);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}

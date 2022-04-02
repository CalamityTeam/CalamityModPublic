using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class RegulusEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regulus Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            int num154 = 14;
            int coolDust;
            projectile.ai[0] += 1;
            if (projectile.ai[0] % 2 == 0)
            {
                if (projectile.ai[0] % 4 == 0)
                {
                    coolDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                }
                else
                {
                    coolDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                }
                Main.dust[coolDust].noGravity = true;
                Main.dust[coolDust].velocity *= 0.1f;
                Main.dust[coolDust].velocity += projectile.velocity * 0.5f;
            }
            if (projectile.ai[0] < 90)
            {
                projectile.velocity.X *= 0.98f;
                projectile.velocity.Y *= 0.98f;
            }
            else
            {
                projectile.extraUpdates = 1;

                Vector2 center = projectile.Center;
                float maxDistance = 500f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * 20f + moveDirection * 12f) / (21f);
                }
                else
                    projectile.Kill();
            }

            projectile.rotation += 0.25f;
        }

        public override bool CanDamage()
        {
            return projectile.ai[0] >= 90;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 24;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, ModContent.DustType<AstralOrange>(), vector7.X * 0.5f, vector7.Y * 0.5f, 100, default, 0.75f);
                Main.dust[num228].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60);
        }
    }
}

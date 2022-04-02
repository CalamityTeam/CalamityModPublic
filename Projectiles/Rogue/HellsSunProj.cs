using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class HellsSunProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/HellsSun";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell's Sun");
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 20;
            projectile.timeLeft = 1200;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;

            if (projectile.ai[0] >= 70f)
            {
                projectile.velocity.X *= 0.96f;
                projectile.velocity.Y *= 0.96f;
                projectile.localAI[1]++;
                if (projectile.Calamity().stealthStrike)
                {
                    if (projectile.localAI[1] >= 30f)
                    {
                        Vector2 velocity = projectile.velocity;
                        Vector2 vector2_1 = new Vector2((float) Main.rand.Next(-100, 101), (float) Main.rand.Next(-100, 101));
                        vector2_1.Normalize();
                        Vector2 vector2_2 = vector2_1 * ((float) Main.rand.Next(10, 41) * 0.1f);
                        if (Main.rand.Next(3) == 0)
                            vector2_2 *= 2f;
                        Vector2 vector2_3 = velocity * 0.25f + vector2_2;
                        Vector2 vector2_5 = vector2_3 * 0.8f;
                        Projectile.NewProjectile(projectile.Center.X - vector2_5.X, projectile.Center.Y - vector2_5.Y, vector2_5.X, vector2_5.Y, ModContent.ProjectileType<ExplosiveSun>(), projectile.damage, projectile.knockBack, projectile.owner, Main.rand.Next(3), 0.0f);
                        projectile.localAI[1] = 0.0f;
                    }
                }
                else
                {
                    if (projectile.localAI[1] >= 60f)
                    {
                        Vector2 velocity = projectile.velocity;
                        Vector2 vector2_1 = new Vector2((float) Main.rand.Next(-100, 101), (float) Main.rand.Next(-100, 101));
                        vector2_1.Normalize();
                        Vector2 vector2_2 = vector2_1 * ((float) Main.rand.Next(10, 41) * 0.1f);
                        if (Main.rand.Next(3) == 0)
                            vector2_2 *= 2f;
                        Vector2 vector2_3 = velocity * 0.25f + vector2_2;
                        Vector2 vector2_5 = vector2_3 * 0.8f;
                        Projectile.NewProjectile(projectile.Center.X - vector2_5.X, projectile.Center.Y - vector2_5.Y, vector2_5.X, vector2_5.Y, ModContent.ProjectileType<ExplosiveSun>(), projectile.damage, projectile.knockBack, projectile.owner, Main.rand.Next(3), 0.0f);
                        projectile.localAI[1] = 0.0f;
                    }
                }
            }
            else
            {
                projectile.rotation += 0.3f * (float)projectile.direction;
            }

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls == true)
            {
                projectile.active = false;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}

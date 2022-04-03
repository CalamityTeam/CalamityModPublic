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
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.Calamity().rogue = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 20;
            Projectile.timeLeft = 1200;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 70f)
            {
                Projectile.velocity.X *= 0.96f;
                Projectile.velocity.Y *= 0.96f;
                Projectile.localAI[1]++;
                if (Projectile.Calamity().stealthStrike)
                {
                    if (Projectile.localAI[1] >= 30f)
                    {
                        Vector2 velocity = Projectile.velocity;
                        Vector2 vector2_1 = new Vector2((float) Main.rand.Next(-100, 101), (float) Main.rand.Next(-100, 101));
                        vector2_1.Normalize();
                        Vector2 vector2_2 = vector2_1 * ((float) Main.rand.Next(10, 41) * 0.1f);
                        if (Main.rand.Next(3) == 0)
                            vector2_2 *= 2f;
                        Vector2 vector2_3 = velocity * 0.25f + vector2_2;
                        Vector2 vector2_5 = vector2_3 * 0.8f;
                        Projectile.NewProjectile(Projectile.Center.X - vector2_5.X, Projectile.Center.Y - vector2_5.Y, vector2_5.X, vector2_5.Y, ModContent.ProjectileType<ExplosiveSun>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(3), 0.0f);
                        Projectile.localAI[1] = 0.0f;
                    }
                }
                else
                {
                    if (Projectile.localAI[1] >= 60f)
                    {
                        Vector2 velocity = Projectile.velocity;
                        Vector2 vector2_1 = new Vector2((float) Main.rand.Next(-100, 101), (float) Main.rand.Next(-100, 101));
                        vector2_1.Normalize();
                        Vector2 vector2_2 = vector2_1 * ((float) Main.rand.Next(10, 41) * 0.1f);
                        if (Main.rand.Next(3) == 0)
                            vector2_2 *= 2f;
                        Vector2 vector2_3 = velocity * 0.25f + vector2_2;
                        Vector2 vector2_5 = vector2_3 * 0.8f;
                        Projectile.NewProjectile(Projectile.Center.X - vector2_5.X, Projectile.Center.Y - vector2_5.Y, vector2_5.X, vector2_5.Y, ModContent.ProjectileType<ExplosiveSun>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(3), 0.0f);
                        Projectile.localAI[1] = 0.0f;
                    }
                }
            }
            else
            {
                Projectile.rotation += 0.3f * (float)Projectile.direction;
            }

            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls == true)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
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

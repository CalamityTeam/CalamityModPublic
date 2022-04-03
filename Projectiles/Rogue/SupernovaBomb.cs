using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SupernovaBomb : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Supernova";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supernova Bomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = true;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //dust and lighting
            int num298 = Main.rand.NextBool(2) ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                num298 = 269;
            }
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, num298, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Lighting.AddLight(Projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);

            //velocity
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Math.Abs(Projectile.velocity.X) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y += 0.2f;
            }

            //rotation
            Projectile.rotation += Projectile.velocity.X * 0.1f;

            //stealth strike
            if (Projectile.Calamity().stealthStrike && Projectile.timeLeft % 8 == 0 && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.Center, Vector2.UnitY * 2f, ModContent.ProjectileType<SupernovaHoming>(), (int)(Projectile.damage * 0.48), Projectile.knockBack, Projectile.owner, 0f, 0f);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 128;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            //spawn projectiles
            int projAmt = Main.rand.Next(3, 5);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                for (int i = 0; i < projAmt; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<SupernovaSpike>(), (int)(Projectile.damage * 0.6), 0f, Projectile.owner, 0f, 0f);
                }
                float spread = MathHelper.Pi / 3f;
                double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 6f;
                double offsetAngle;
                for (int i = 0; i < 3; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<SupernovaHoming>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<SupernovaHoming>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
            }

            //dust effects
            int dustType = Main.rand.NextBool(2) ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                dustType = 269;
            }
            for (int d = 0; d < 5; d++)
            {
                int exo = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[exo].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[exo].scale = 0.5f;
                    Main.dust[exo].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 9; d++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }

            //gore cloud effects
            CalamityUtils.ExplosionGores(Projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

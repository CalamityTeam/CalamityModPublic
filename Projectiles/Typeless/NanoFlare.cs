using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class NanoFlare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nano Flare");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 60;
        }

        public override void AI()
        {
            //Dust
            for (int i = 0; i< 3; i++)
            {
                int dint = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 0, default, 0.75f);
                Dust dust = Main.dust[dint];
                dust.velocity = projectile.velocity * Main.rand.NextFloat(-0.3f,0.3f);
                dust.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, 6f));
                int dint = Dust.NewDust(projectile.Center, 1, 1, 107, dspeed.X, dspeed.Y, 0, default, 0.7f);
                Dust dust = Main.dust[dint];
                dust.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
            }
            Main.PlaySound(SoundID.Item14, projectile.Center);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.width = 60;
            projectile.height = 60;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            Vector2 goreVec = new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f);
            float smokeScale = 0.66f;
            for (int i = 0; i < 2; i++)
            {
                switch (Main.rand.Next(1, 5))
                {
                    case 1: int idx1 = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx1].velocity *= smokeScale;
                            Main.gore[idx1].velocity.X += 1f;
                            Main.gore[idx1].velocity.Y += 1f;
                            break;

                    case 2: int idx2 = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx2].velocity *= smokeScale;
                            Main.gore[idx2].velocity.X -= 1f;
                            Main.gore[idx2].velocity.Y += 1f;
                            break;

                    case 3: int idx3 = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx3].velocity *= smokeScale;
                            Main.gore[idx3].velocity.X += 1f;
                            Main.gore[idx3].velocity.Y -= 1f;
                            break;

                    case 4: int idx4 = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx4].velocity *= smokeScale;
                            Main.gore[idx4].velocity.X -= 1f;
                            Main.gore[idx4].velocity.Y -= 1f;
                            break;
                    default: break;
                }
            }
        }
    }
}

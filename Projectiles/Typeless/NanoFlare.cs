using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class NanoFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            //Dust
            for (int i = 0; i< 3; i++)
            {
                int dint = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 0, default, 0.75f);
                Dust dust = Main.dust[dint];
                dust.velocity = Projectile.velocity * Main.rand.NextFloat(-0.3f,0.3f);
                dust.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, 6f));
                int dint = Dust.NewDust(Projectile.Center, 1, 1, 107, dspeed.X, dspeed.Y, 0, default, 0.7f);
                Dust dust = Main.dust[dint];
                dust.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreVec = new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f);
                float smokeScale = 0.66f;
                for (int i = 0; i < 2; i++)
                {
                    switch (Main.rand.Next(1, 5))
                    {
                        case 1:
                            int idx1 = Gore.NewGore(Projectile.GetSource_Death(), goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx1].velocity *= smokeScale;
                            Main.gore[idx1].velocity.X += 1f;
                            Main.gore[idx1].velocity.Y += 1f;
                            break;

                        case 2:
                            int idx2 = Gore.NewGore(Projectile.GetSource_Death(), goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx2].velocity *= smokeScale;
                            Main.gore[idx2].velocity.X -= 1f;
                            Main.gore[idx2].velocity.Y += 1f;
                            break;

                        case 3:
                            int idx3 = Gore.NewGore(Projectile.GetSource_Death(), goreVec, default, Main.rand.Next(61, 64), 1f);
                            Main.gore[idx3].velocity *= smokeScale;
                            Main.gore[idx3].velocity.X += 1f;
                            Main.gore[idx3].velocity.Y -= 1f;
                            break;

                        case 4:
                            int idx4 = Gore.NewGore(Projectile.GetSource_Death(), goreVec, default, Main.rand.Next(61, 64), 1f);
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
}

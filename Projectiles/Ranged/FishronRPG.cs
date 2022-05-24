using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class FishronRPG : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("RPG");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 95;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 200;
                Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.knockBack = 10f;
            }
            else
            {
                if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
                {
                    for (int num246 = 0; num246 < 2; num246++)
                    {
                        float num247 = 0f;
                        float num248 = 0f;
                        if (num246 == 1)
                        {
                            num247 = Projectile.velocity.X * 0.5f;
                            num248 = Projectile.velocity.Y * 0.5f;
                        }
                        int num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 33, 0f, 0f, 100, default, 1f);
                        Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                        Main.dust[num249].velocity *= 0.2f;
                        Main.dust[num249].noGravity = true;
                        num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 33, 0f, 0f, 100, default, 0.5f);
                        Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[num249].velocity *= 0.05f;
                    }
                }
            }
            CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig1, Projectile.position, 1f, 0f);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 48;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 20; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}

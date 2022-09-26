using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class ScourgeoftheCosmosProj : ModProjectile
    {
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Scourge");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.alpha <= 200)
            {
                int num3;
                for (int num20 = 0; num20 < 2; num20 = num3 + 1)
                {
                    int dustType = Main.rand.NextBool(3) ? 56 : 242;
                    float num21 = Projectile.velocity.X / 4f * num20;
                    float num22 = Projectile.velocity.Y / 4f * num20;
                    int num23 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                    Main.dust[num23].position.X = Projectile.Center.X - num21;
                    Main.dust[num23].position.Y = Projectile.Center.Y - num22;
                    Dust dust = Main.dust[num23];
                    dust.velocity *= 0f;
                    Main.dust[num23].scale = 0.7f;
                    num3 = num20;
                }
            }
            Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 0.785f;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 180f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
            }
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                Projectile.Kill();
            else
            {
                SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.position);
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                if (Projectile.owner == Main.myPlayer)
                {
                    int num626 = 1;
                    if (Main.rand.NextBool(10))
                        num626++;
                    int num3;
                    for (int num627 = 0; num627 < num626; num627 = num3 + 1)
                    {
                        float num628 = Main.rand.Next(-35, 36) * 0.02f;
                        float num629 = Main.rand.Next(-35, 36) * 0.02f;
                        num628 *= 10f;
                        num629 *= 10f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, num628, num629, ModContent.ProjectileType<ScourgeoftheCosmosMini>(), (int)(Projectile.damage * 0.7), Projectile.knockBack * 0.35f, Main.myPlayer);
                        num3 = num627;
                    }
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.position);
            int num3;
            for (int num622 = 0; num622 < 10; num622 = num3 + 1)
            {
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                int num623 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num623];
                dust.scale *= 1.1f;
                Main.dust[num623].noGravity = true;
                num3 = num622;
            }
            for (int num624 = 0; num624 < 15; num624 = num3 + 1)
            {
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                int num625 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num625];
                dust.velocity *= 2.5f;
                dust = Main.dust[num625];
                dust.scale *= 0.8f;
                Main.dust[num625].noGravity = true;
                num3 = num624;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int num626 = 3;
                if (Main.rand.NextBool(10))
                    num626++;
                for (int num627 = 0; num627 < num626; num627 = num3 + 1)
                {
                    float num628 = Main.rand.Next(-35, 36) * 0.02f;
                    float num629 = Main.rand.Next(-35, 36) * 0.02f;
                    num628 *= 10f;
                    num629 *= 10f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, num628, num629, ModContent.ProjectileType<ScourgeoftheCosmosMini>(), (int)(Projectile.damage * 0.7), Projectile.knockBack * 0.35f, Main.myPlayer);
                    num3 = num627;
                }
            }
        }
    }
}

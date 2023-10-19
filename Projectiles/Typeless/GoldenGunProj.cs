using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GoldenGunProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 5f)
            {
                Projectile.localAI[0] += 1f;
                return;
            }
            int inc;
            for (int i = 0; i < 1; i = inc + 1)
            {
                for (int j = 0; j < 6; j = inc + 1)
                {
                    float dustX = Projectile.velocity.X / 6f * (float)j;
                    float dustY = Projectile.velocity.Y / 6f * (float)j;
                    int dustPosMod = 6;
                    int goldenDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)dustPosMod, Projectile.position.Y + (float)dustPosMod), Projectile.width - dustPosMod * 2, Projectile.height - dustPosMod * 2, 170, 0f, 0f, 75, default, 1.2f);
                    Dust dust = Main.dust[goldenDust];
                    if (Main.rand.NextBool())
                    {
                        dust.alpha += 25;
                    }
                    if (Main.rand.NextBool())
                    {
                        dust.alpha += 25;
                    }
                    if (Main.rand.NextBool())
                    {
                        dust.alpha += 25;
                    }
                    dust.noGravity = true;
                    dust.velocity *= 0.3f;
                    dust.velocity += Projectile.velocity * 0.5f;
                    dust.position = Projectile.Center;
                    dust.position.X -= dustX;
                    dust.position.Y -= dustY;
                    dust.velocity *= 0.2f;
                    inc = j;
                }
                if (Main.rand.NextBool(4))
                {
                    int dustPosMod2 = 6;
                    int moreGoldenDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)dustPosMod2, Projectile.position.Y + (float)dustPosMod2), Projectile.width - dustPosMod2 * 2, Projectile.height - dustPosMod2 * 2, 170, 0f, 0f, 75, default, 0.65f);
                    Dust dust = Main.dust[moreGoldenDust];
                    dust.velocity *= 0.5f;
                    dust.velocity += Projectile.velocity * 0.5f;
                }
                inc = i;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.velocity = Projectile.oldVelocity * 0.2f;
            int inc;
            for (int k = 0; k < 100; k = inc + 1)
            {
                int deathGoldenDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 170, 0f, 0f, 75, default, 1.2f);
                Dust dust = Main.dust[deathGoldenDust];
                if (Main.rand.NextBool())
                {
                    dust.alpha += 25;
                }
                if (Main.rand.NextBool())
                {
                    dust.alpha += 25;
                }
                if (Main.rand.NextBool())
                {
                    dust.alpha += 25;
                }
                if (Main.rand.NextBool())
                {
                    dust.scale = 0.6f;
                }
                else
                {
                    dust.noGravity = true;
                }
                dust.velocity *= 0.3f;
                dust.velocity += Projectile.velocity;
                dust.velocity *= 1f + (float)Main.rand.Next(-100, 101) * 0.01f;
                dust.velocity.X += (float)Main.rand.Next(-50, 51) * 0.015f;
                dust.velocity.Y += (float)Main.rand.Next(-50, 51) * 0.015f;
                dust.position = Projectile.Center;
                inc = k;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Ichor, 480);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Ichor, 480);
    }
}

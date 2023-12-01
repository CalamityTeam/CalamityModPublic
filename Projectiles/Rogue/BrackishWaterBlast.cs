using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrackishWaterBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.75f / 255f);
            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                33,
                89
            });
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            bool xflag = false;
            bool yflag = false;
            if (Projectile.velocity.X < 0f && Projectile.position.X < Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.X > 0f && Projectile.position.X > Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.Y < 0f && Projectile.position.Y < Projectile.ai[1])
            {
                yflag = true;
            }
            if (Projectile.velocity.Y > 0f && Projectile.position.Y > Projectile.ai[1])
            {
                yflag = true;
            }
            if (xflag && yflag)
            {
                Projectile.Kill();
            }
            float projTimer = 25f;
            if (Projectile.ai[0] > 180f)
            {
                projTimer -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;
            Projectile.ai[0] += 4f;
            float timerCompare = 0f;
            while (timerCompare < projTimer)
            {
                float random1 = (float)Main.rand.Next(-10, 11);
                float random2 = (float)Main.rand.Next(-10, 11);
                float random3 = (float)Main.rand.Next(3, 9);
                float randomAdjust = (float)Math.Sqrt((double)(random1 * random1 + random2 * random2));
                randomAdjust = random3 / randomAdjust;
                random1 *= randomAdjust;
                random2 *= randomAdjust;
                int water = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 0.5f);
                Dust dust = Main.dust[water];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = random1;
                dust.velocity.Y = random2;
                timerCompare++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 6;
            target.AddBuff(BuffID.Venom, 90);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 90);
        }
    }
}

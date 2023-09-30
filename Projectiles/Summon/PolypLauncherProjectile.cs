using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class PolypLauncherProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float Gravity = 0.4f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y <= 10f)
            {
                Projectile.velocity.Y += Gravity;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 10)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255, 0, Projectile.ai[0] / 10f);
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.1f * Projectile.direction;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 225, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.15f, 120, default, 1f);
                dust_splash += 1;
            }
            int split = 0;
            int shardAmt = Main.rand.Next(1,5);
            while (split < shardAmt)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -Projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -Projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -Projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -Projectile.velocity.Y;
                }
                Vector2 shardSpeed = new Vector2(shardspeedX, shardspeedY);

                //Spawn the projectile
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + shardSpeed, shardSpeed, ModContent.ProjectileType<PolypLauncherShrapnel>(), (int)(Projectile.damage * 0.5), Projectile.knockBack / 2f, Projectile.owner, Main.rand.Next(3));
                split += 1;
            }
        }
    }
}

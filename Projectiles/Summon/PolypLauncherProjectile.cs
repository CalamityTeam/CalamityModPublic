using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PolypLauncherProjectile : ModProjectile
    {
        public const float Gravity = 0.4f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polyp Chunk");
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.minion = true;
            projectile.minionSlots = 0f;
        }

        public override void AI()
        {
            if (projectile.velocity.Y <= 10f)
            {
                projectile.velocity.Y += Gravity;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] < 10)
            {
                projectile.alpha = (int)MathHelper.Lerp(255, 0, projectile.ai[0] / 10f);
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.1f * projectile.direction;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.Center);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 225, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.15f, 120, default, 1f);
                dust_splash += 1;
            }
            int split = 0;
            int shardAmt = Main.rand.Next(1,5);
            while (split < shardAmt)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -projectile.velocity.Y;
                }
                Vector2 shardSpeed = new Vector2(shardspeedX, shardspeedY);

                //Spawn the projectile
                int shard = Projectile.NewProjectile(projectile.Center + shardSpeed, shardSpeed, ModContent.ProjectileType<PolypLauncherShrapnel>(), (int)(projectile.damage * 0.5), projectile.knockBack / 2f, projectile.owner, Main.rand.Next(3));
                split += 1;
            }
        }
    }
}

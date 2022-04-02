using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Healing
{
    class PhantomicHeart : ModProjectile
    {
        private int floatTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantomic Heart");
            Main.projFrames[projectile.type] = 4;

        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.alpha = 20;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(floatTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            floatTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            if (floatTimer >= 10)
            {
                projectile.velocity.Y *= 0.99f;
            }
            else
            {
                projectile.velocity.Y *= 1.01f;
            }
            if (floatTimer >= 20)
                floatTimer = 0;
            else
                floatTimer++;

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(projectile.TopLeft, projectile.width, projectile.height, 5, Main.rand.NextFloat(-3, 3), -5f, 0, new Color(99, 54, 84), Main.rand.NextFloat(0.5f, 1.5f));
            }

            Player player = Main.player[projectile.owner];
            Vector2 playerVector = player.Center - projectile.Center;
            float playerDist = playerVector.Length();
            if (projectile.timeLeft < 500 && playerDist < 50f && projectile.position.X < player.position.X + player.width && projectile.position.X + projectile.width > player.position.X && projectile.position.Y < player.position.Y + player.height && projectile.position.Y + projectile.height > player.position.Y)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    CalamityPlayer calPlayer = player.Calamity();
                    calPlayer.phantomicHeartRegen = 720;
                    projectile.Kill();
                }
            }
            if (player.lifeMagnet && projectile.timeLeft < 510)
            {
                float N = 18f;
                playerDist = 15f / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
                projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame % 2 == 0)
                    projectile.netUpdate = true;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }
    }
}

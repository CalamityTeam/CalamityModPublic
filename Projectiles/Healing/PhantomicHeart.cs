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
            Main.projFrames[Projectile.type] = 4;

        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.alpha = 20;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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
                Projectile.velocity.Y *= 0.99f;
            }
            else
            {
                Projectile.velocity.Y *= 1.01f;
            }
            if (floatTimer >= 20)
                floatTimer = 0;
            else
                floatTimer++;

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, 5, Main.rand.NextFloat(-3, 3), -5f, 0, new Color(99, 54, 84), Main.rand.NextFloat(0.5f, 1.5f));
            }

            Player player = Main.player[Projectile.owner];
            Vector2 playerVector = player.Center - Projectile.Center;
            float playerDist = playerVector.Length();
            if (Projectile.timeLeft < 500 && playerDist < 50f && Projectile.position.X < player.position.X + player.width && Projectile.position.X + Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + player.height && Projectile.position.Y + Projectile.height > player.position.Y)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    CalamityPlayer calPlayer = player.Calamity();
                    calPlayer.phantomicHeartRegen = 720;
                    Projectile.Kill();
                }
            }
            if (player.lifeMagnet && Projectile.timeLeft < 510)
            {
                float N = 18f;
                playerDist = 15f / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                Projectile.velocity.X = (Projectile.velocity.X * N + playerVector.X) / (N + 1f);
                Projectile.velocity.Y = (Projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame % 2 == 0)
                    Projectile.netUpdate = true;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
    }
}

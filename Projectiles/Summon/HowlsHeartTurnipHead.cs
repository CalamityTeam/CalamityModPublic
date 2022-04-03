using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class HowlsHeartTurnipHead : ModProjectile
    {
        private bool fly = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Turnip-Head");
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 70;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();
            fallThrough = playerDistance > 200f;
            return true;
        }

        public override void AI()
        {            
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //If the correct minion, set bools
            bool correctMinion = Projectile.type == ModContent.ProjectileType<HowlsHeartTurnipHead>();
            if (!modPlayer.howlsHeart && !modPlayer.howlsHeartVanity || !player.active)
            {
                Projectile.active = false;
                return;
            }
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.howlTrio = false;
                }
                if (modPlayer.howlTrio)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Vector2 vector46 = Projectile.position;
            if (!fly)
            {
                Projectile.rotation = 0;
                Vector2 playerVec = player.Center - Projectile.Center;
                float playerDistance = playerVec.Length();
                if (Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.Y = -3f;
                }
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 7f)
                {
                    Projectile.velocity.Y = 7f;
                }
                if (playerDistance > 600f)
                {
                    fly = true;
                    Projectile.velocity.X = 0f;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.10f;
                        if (Projectile.velocity.X > 7f)
                        {
                            Projectile.velocity.X = 7f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.10f;
                        if (Projectile.velocity.X < -7f)
                        {
                            Projectile.velocity.X = -7f;
                        }
                    }
                }
                if (playerDistance < 100f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.5f)
                        {
                            Projectile.velocity.X -= 0.15f;
                        }
                        else if (Projectile.velocity.X < -0.5f)
                        {
                            Projectile.velocity.X += 0.15f;
                        }
                        else if (Projectile.velocity.X < 0.5f && Projectile.velocity.X > -0.5f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }
            }
            else if (fly)
            {
                //still hopping
                if (Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.Y = -3f;
                }
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 7f)
                {
                    Projectile.velocity.Y = 7f;
                }
                Projectile.alpha += 15;
                if (Projectile.alpha >= 255)
                {
                    Projectile.position.X = player.position.X;
                    Projectile.position.Y = player.position.Y - 5f;
                    fly = false;
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.velocity.X > 0.25f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public override bool? CanDamage() => false;
    }
}

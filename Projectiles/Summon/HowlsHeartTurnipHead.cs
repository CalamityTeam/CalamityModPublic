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
            Main.projFrames[projectile.type] = 5;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 70;
            projectile.height = 82;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            Player player = Main.player[projectile.owner];
            Vector2 center2 = projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();
            fallThrough = playerDistance > 200f;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //If the correct minion, set bools
            bool correctMinion = projectile.type == ModContent.ProjectileType<HowlsHeartTurnipHead>();
            if (!modPlayer.howlsHeart && !modPlayer.howlsHeartVanity || !player.active)
            {
                projectile.active = false;
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
                    projectile.timeLeft = 2;
                }
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            Vector2 vector46 = projectile.position;
            if (!fly)
            {
                projectile.rotation = 0;
                Vector2 playerVec = player.Center - projectile.Center;
                float playerDistance = playerVec.Length();
                if (projectile.velocity.Y == 0f)
                {
                    projectile.velocity.Y = -3f;
                }
                projectile.velocity.Y += 0.2f;
                if (projectile.velocity.Y > 7f)
                {
                    projectile.velocity.Y = 7f;
                }
                if (playerDistance > 600f)
                {
                    fly = true;
                    projectile.velocity.X = 0f;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - projectile.position.X > 0f)
                    {
                        projectile.velocity.X += 0.10f;
                        if (projectile.velocity.X > 7f)
                        {
                            projectile.velocity.X = 7f;
                        }
                    }
                    else
                    {
                        projectile.velocity.X -= 0.10f;
                        if (projectile.velocity.X < -7f)
                        {
                            projectile.velocity.X = -7f;
                        }
                    }
                }
                if (playerDistance < 100f)
                {
                    if (projectile.velocity.X != 0f)
                    {
                        if (projectile.velocity.X > 0.5f)
                        {
                            projectile.velocity.X -= 0.15f;
                        }
                        else if (projectile.velocity.X < -0.5f)
                        {
                            projectile.velocity.X += 0.15f;
                        }
                        else if (projectile.velocity.X < 0.5f && projectile.velocity.X > -0.5f)
                        {
                            projectile.velocity.X = 0f;
                        }
                    }
                }
            }
            else if (fly)
            {
                //still hopping
                if (projectile.velocity.Y == 0f)
                {
                    projectile.velocity.Y = -3f;
                }
                projectile.velocity.Y += 0.2f;
                if (projectile.velocity.Y > 7f)
                {
                    projectile.velocity.Y = 7f;
                }
                projectile.alpha += 15;
                if (projectile.alpha >= 255)
                {
                    projectile.position.X = player.position.X;
                    projectile.position.Y = player.position.Y - 5f;
                    fly = false;
                    projectile.alpha = 0;
                }
            }
            if (projectile.velocity.X > 0.25f)
            {
                projectile.spriteDirection = -1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.spriteDirection = 1;
            }
        }

        public override bool CanDamage() => false;
    }
}

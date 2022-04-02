using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class PineapplePetProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pineapple");
            Main.projFrames[projectile.type] = 5;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.pineapplePet = false;
            }
            if (modPlayer.pineapplePet)
            {
                projectile.timeLeft = 2;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }

            float passiveMvtFloat = 0.1f;
            projectile.tileCollide = false;
            float range = 200f;
            float xDist = player.Center.X - projectile.Center.X - 2f;
            float yDist = player.Center.Y - projectile.Center.Y - 60f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 7f;
            if (playerDist < range && player.velocity.Y == 0f && (projectile.position.Y + projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height)))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                    projectile.velocity.Y = -6f;
            }

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - projectile.width / 2;
                projectile.position.Y = player.Center.Y - projectile.height / 2;
                projectile.netUpdate = true;
            }

            if (playerDist < 4f)
            {
                projectile.velocity.X = xDist;
                projectile.velocity.Y = yDist;
                passiveMvtFloat = 0f;
            }
            else
            {
                if (playerDist > 350f)
                {
                    passiveMvtFloat = 0.2f;
                    returnSpeed = 12f;
                }
                float speedMult = returnSpeed / playerDist;
                xDist *= speedMult;
                yDist *= speedMult;
            }
            if (projectile.velocity.X < xDist)
            {
                projectile.velocity.X += passiveMvtFloat;
                if (projectile.velocity.X < 0f)
                    projectile.velocity.X += passiveMvtFloat;
            }
            if (projectile.velocity.X > xDist)
            {
                projectile.velocity.X -= passiveMvtFloat;
                if (projectile.velocity.X > 0f)
                    projectile.velocity.X -= passiveMvtFloat;
            }
            if (projectile.velocity.Y < yDist)
            {
                projectile.velocity.Y += passiveMvtFloat;
                if (projectile.velocity.Y < 0f)
                    projectile.velocity.Y += passiveMvtFloat;
            }
            if (projectile.velocity.Y > yDist)
            {
                projectile.velocity.Y -= passiveMvtFloat;
                if (projectile.velocity.Y > 0f)
                    projectile.velocity.Y -= passiveMvtFloat;
            }
            projectile.direction = -player.direction;
            projectile.spriteDirection = 1;
            projectile.rotation = projectile.velocity.Y * 0.05f * -projectile.direction;
        }
    }
}

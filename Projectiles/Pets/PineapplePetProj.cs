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
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.pineapplePet = false;
            }
            if (modPlayer.pineapplePet)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }

            float passiveMvtFloat = 0.1f;
            Projectile.tileCollide = false;
            float range = 200f;
            float xDist = player.Center.X - Projectile.Center.X - 2f;
            float yDist = player.Center.Y - Projectile.Center.Y - 60f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 7f;
            if (playerDist < range && player.velocity.Y == 0f && (Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                    Projectile.velocity.Y = -6f;
            }

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                Projectile.position.X = player.Center.X - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - Projectile.height / 2;
                Projectile.netUpdate = true;
            }

            if (playerDist < 4f)
            {
                Projectile.velocity.X = xDist;
                Projectile.velocity.Y = yDist;
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
            if (Projectile.velocity.X < xDist)
            {
                Projectile.velocity.X += passiveMvtFloat;
                if (Projectile.velocity.X < 0f)
                    Projectile.velocity.X += passiveMvtFloat;
            }
            if (Projectile.velocity.X > xDist)
            {
                Projectile.velocity.X -= passiveMvtFloat;
                if (Projectile.velocity.X > 0f)
                    Projectile.velocity.X -= passiveMvtFloat;
            }
            if (Projectile.velocity.Y < yDist)
            {
                Projectile.velocity.Y += passiveMvtFloat;
                if (Projectile.velocity.Y < 0f)
                    Projectile.velocity.Y += passiveMvtFloat;
            }
            if (Projectile.velocity.Y > yDist)
            {
                Projectile.velocity.Y -= passiveMvtFloat;
                if (Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y -= passiveMvtFloat;
            }
            Projectile.direction = -player.direction;
            Projectile.spriteDirection = 1;
            Projectile.rotation = Projectile.velocity.Y * 0.05f * -Projectile.direction;
        }
    }
}

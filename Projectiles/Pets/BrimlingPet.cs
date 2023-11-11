using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class BrimlingPet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 62;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.brimling = false;
            }
            if (modPlayer.brimling)
            {
                Projectile.timeLeft = 4;
            }
            float flySpeed = 0.5f;
            Projectile.tileCollide = false;
            Vector2 flyDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - flyDirection.X;
            float vertPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - flyDirection.Y;
            vertPos += (float)Main.rand.Next(-10, 21);
            horiPos += (float)Main.rand.Next(-10, 21);
            horiPos += (float)(60 * -(float)Main.player[Projectile.owner].direction);
            vertPos -= 60f;
            float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertPos * vertPos));

            //Limites de mouvement ici

            if (playerDistance > 1000f)
            {
                Projectile.position.X = Projectile.position.X + horiPos;
                Projectile.position.Y = Projectile.position.Y + vertPos;
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0, -1f, 0, default, 1f);
                }
            }

            if (playerDistance < 100f && Main.player[Projectile.owner].velocity.Y == 0f &&
                Projectile.position.Y + (float)Projectile.height <= Main.player[Projectile.owner].position.Y + (float)Main.player[Projectile.owner].height &&
                !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }
            if (playerDistance < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.90f;
                }
                flySpeed = 0.01f;
            }
            else
            {
                if (playerDistance < 100f)
                {
                    flySpeed = 0.1f;
                }
                if (playerDistance > 300f)
                {
                    flySpeed = 1f;
                }
                playerDistance = 18f / playerDistance;
                horiPos *= playerDistance;
                vertPos *= playerDistance;
            }

            //Les changements de velocité ici
            if (Projectile.velocity.X < horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X + flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + flySpeed;
                }
            }
            if (Projectile.velocity.X > horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X - flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - flySpeed;
                }
            }
            if (Projectile.velocity.Y < vertPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + flySpeed * 2f;
                }
            }
            if (Projectile.velocity.Y > vertPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - flySpeed * 2f;
                }
            }
            if ((double)Projectile.velocity.X > 0.25)
            {
                Projectile.direction = -1;
            }
            else if ((double)Projectile.velocity.X < -0.25)
            {
                Projectile.direction = 1;
            }

            //On gère le sprite et les frames ici

            Player projOwner = Main.player[Projectile.owner];

            Projectile.spriteDirection = -projOwner.direction;
            Projectile.rotation = Projectile.velocity.X * 0.03f;

            Projectile.frameCounter++;

            if (projOwner.statLife >= projOwner.statLifeMax2 / 4)
            {
                if (Projectile.frameCounter > 5)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    return;
                }
            }
            else
            {
                if (Projectile.frameCounter > 5)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 4;
                    return;
                }
            }
        }
    }
}

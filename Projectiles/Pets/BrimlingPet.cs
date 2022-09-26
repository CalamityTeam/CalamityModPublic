using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class BrimlingPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimling");
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
            float num16 = 0.5f;
            Projectile.tileCollide = false;
            int num17 = 100;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num18 = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector3.X;
            float num19 = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector3.Y;
            num19 += (float)Main.rand.Next(-10, 21);
            num18 += (float)Main.rand.Next(-10, 21);
            num18 += (float)(60 * -(float)Main.player[Projectile.owner].direction);
            num19 -= 60f;
            float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            float num21 = 18f;

            //Limites de mouvement ici

            float num27 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            if (num27 > 1000f)
            {
                Projectile.position.X = Projectile.position.X + num18;
                Projectile.position.Y = Projectile.position.Y + num19;
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0, -1f, 0, default, 1f);
                }
            }

            if (num20 < (float)num17 && Main.player[Projectile.owner].velocity.Y == 0f &&
                Projectile.position.Y + (float)Projectile.height <= Main.player[Projectile.owner].position.Y + (float)Main.player[Projectile.owner].height &&
                !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }
            if (num20 < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.90f;
                }
                num16 = 0.01f;
            }
            else
            {
                if (num20 < 100f)
                {
                    num16 = 0.1f;
                }
                if (num20 > 300f)
                {
                    num16 = 1f;
                }
                num20 = num21 / num20;
                num18 *= num20;
                num19 *= num20;
            }

            //Les changements de velocité ici
            if (Projectile.velocity.X < num18)
            {
                Projectile.velocity.X = Projectile.velocity.X + num16;
                if (num16 > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num16;
                }
            }
            if (Projectile.velocity.X > num18)
            {
                Projectile.velocity.X = Projectile.velocity.X - num16;
                if (num16 > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num16;
                }
            }
            if (Projectile.velocity.Y < num19)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num16;
                if (num16 > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num16 * 2f;
                }
            }
            if (Projectile.velocity.Y > num19)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num16;
                if (num16 > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num16 * 2f;
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

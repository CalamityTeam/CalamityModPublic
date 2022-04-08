using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class BendyPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreamfog Dragon");
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 38;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Set up namespaces
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //If the player dies or something, kill thte projectile
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.bendyPet = false;
            }

            //Prevent the projectile from losing its timeLeft
            if (modPlayer.bendyPet)
            {
                Projectile.timeLeft = 2;
            }

            //Update frames
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

            //Highlight nearby danger sources
            player.dangerSense = true;
            player.detectCreature = true;

            //Generate less light if returning to the player
            if (Projectile.localAI[0] == 0f)
            {
                Lighting.AddLight(Projectile.Center, 0.8118f, 0.2529f, 1.3294f); //138, 43, 226
            }
            else
            {
                Lighting.AddLight(Projectile.Center, 0.4329f, 0.1349f, 0.709f);
            }

            float idleMvt = 0.2f;
            float projSpeed = 5f;
            //Calculate where the pet should reside based on pressed controls
            Vector2 projVec = player.Center - Projectile.Center;
            projVec.Y += player.gfxOffY;
            if (player.controlLeft)
            {
                projVec.X -= 120f;
            }
            else if (player.controlRight)
            {
                projVec.X += 120f;
            }
            if (player.controlDown)
            {
                projVec.Y += 120f;
            }
            else
            {
                if (player.controlUp)
                {
                    projVec.Y -= 120f;
                }
                projVec.Y -= 60f;
            }
            float playerDist = projVec.Length();
            if (playerDist > 1000f)
            {
                Projectile.position.X += projVec.X;
                Projectile.position.Y += projVec.Y;
            }

            //Returning to player AI
            if (Projectile.localAI[0] == 1f)
            {
                //If close enough, the player isn't moving too much, and isn't moving vertically, return to normal
                if (playerDist < 10f && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) < projSpeed && player.velocity.Y == 0f)
                {
                    Projectile.localAI[0] = 0f;
                }

                //Go faster
                projSpeed = 12f;
                if (playerDist < projSpeed)
                {
                    Projectile.velocity.X = projVec.X;
                    Projectile.velocity.Y = projVec.Y;
                }
                else
                {
                    playerDist = projSpeed / playerDist;
                    Projectile.velocity.X = projVec.X * playerDist;
                    Projectile.velocity.Y = projVec.Y * playerDist;
                }

                //Set projectile direction and rotation
                if (Projectile.velocity.X > 0.5f)
                {
                    Projectile.direction = -1;
                }
                else if (Projectile.velocity.X < -0.5f)
                {
                    Projectile.direction = 1;
                }
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                return;
            }

            //If too far, return to the player
            if (playerDist > 200f)
            {
                Projectile.localAI[0] = 1f;
            }

            //Set projectile direction and rotation
            if (Projectile.velocity.X > 0.5f)
            {
                Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < -0.5f)
            {
                Projectile.direction = 1;
            }
            Projectile.spriteDirection = Projectile.direction;

            if (playerDist < 10f)
            {
                Projectile.velocity.X = projVec.X;
                Projectile.velocity.Y = projVec.Y;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (playerDist < projSpeed)
                {
                    Projectile.position += Projectile.velocity;
                    Projectile.velocity *= 0f;
                    idleMvt = 0f;
                }
                Projectile.direction = -player.direction;
            }
            playerDist = projSpeed / playerDist;
            projVec.X *= playerDist;
            projVec.Y *= playerDist;
            if (Projectile.velocity.X < projVec.X)
            {
                Projectile.velocity.X += idleMvt;
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X *= 0.99f;
                }
            }
            if (Projectile.velocity.X > projVec.X)
            {
                Projectile.velocity.X -= idleMvt;
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X *= 0.99f;
                }
            }
            if (Projectile.velocity.Y < projVec.Y)
            {
                Projectile.velocity.Y += idleMvt;
                if (Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y *= 0.99f;
                }
            }
            if (Projectile.velocity.Y > projVec.Y)
            {
                Projectile.velocity.Y -= idleMvt;
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y *= 0.99f;
                }
            }

            //Rotation if moving
            if (Projectile.velocity.X != 0f || Projectile.velocity.Y != 0f)
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/BendyPetGlow").Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color rainbow = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
            {
                new Color(255, 0, 0, 50), //Red
                new Color(255, 255, 0, 50), //Yellow
                new Color(0, 255, 0, 50), //Green
                new Color(0, 255, 255, 50), //Cyan
                new Color(0, 0, 255, 50), //Blue
                new Color(255, 0, 255, 50), //Fuschia
            });

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), rainbow, Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), Projectile.scale, spriteEffects, 0);
        }
    }
}

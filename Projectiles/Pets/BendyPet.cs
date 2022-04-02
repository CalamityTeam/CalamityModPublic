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
            Main.projFrames[projectile.type] = 5;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Set up namespaces
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //If the player dies or something, kill thte projectile
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.bendyPet = false;
            }

            //Prevent the projectile from losing its timeLeft
            if (modPlayer.bendyPet)
            {
                projectile.timeLeft = 2;
            }

            //Update frames
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

            //Highlight nearby danger sources
            player.dangerSense = true;
            player.detectCreature = true;

            //Generate less light if returning to the player
            if (projectile.localAI[0] == 0f)
            {
                Lighting.AddLight(projectile.Center, 0.8118f, 0.2529f, 1.3294f); //138, 43, 226
            }
            else
            {
                Lighting.AddLight(projectile.Center, 0.4329f, 0.1349f, 0.709f);
            }

            float idleMvt = 0.2f;
            float projSpeed = 5f;
            //Calculate where the pet should reside based on pressed controls
            Vector2 projVec = player.Center - projectile.Center;
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
                projectile.position.X += projVec.X;
                projectile.position.Y += projVec.Y;
            }

            //Returning to player AI
            if (projectile.localAI[0] == 1f)
            {
                //If close enough, the player isn't moving too much, and isn't moving vertically, return to normal
                if (playerDist < 10f && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) < projSpeed && player.velocity.Y == 0f)
                {
                    projectile.localAI[0] = 0f;
                }

                //Go faster
                projSpeed = 12f;
                if (playerDist < projSpeed)
                {
                    projectile.velocity.X = projVec.X;
                    projectile.velocity.Y = projVec.Y;
                }
                else
                {
                    playerDist = projSpeed / playerDist;
                    projectile.velocity.X = projVec.X * playerDist;
                    projectile.velocity.Y = projVec.Y * playerDist;
                }

                //Set projectile direction and rotation
                if (projectile.velocity.X > 0.5f)
                {
                    projectile.direction = -1;
                }
                else if (projectile.velocity.X < -0.5f)
                {
                    projectile.direction = 1;
                }
                projectile.spriteDirection = projectile.direction;
                projectile.rotation = projectile.velocity.X * 0.05f;
                return;
            }

            //If too far, return to the player
            if (playerDist > 200f)
            {
                projectile.localAI[0] = 1f;
            }

            //Set projectile direction and rotation
            if (projectile.velocity.X > 0.5f)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.5f)
            {
                projectile.direction = 1;
            }
            projectile.spriteDirection = projectile.direction;

            if (playerDist < 10f)
            {
                projectile.velocity.X = projVec.X;
                projectile.velocity.Y = projVec.Y;
                projectile.rotation = projectile.velocity.X * 0.05f;
                if (playerDist < projSpeed)
                {
                    projectile.position += projectile.velocity;
                    projectile.velocity *= 0f;
                    idleMvt = 0f;
                }
                projectile.direction = -player.direction;
            }
            playerDist = projSpeed / playerDist;
            projVec.X *= playerDist;
            projVec.Y *= playerDist;
            if (projectile.velocity.X < projVec.X)
            {
                projectile.velocity.X += idleMvt;
                if (projectile.velocity.X < 0f)
                {
                    projectile.velocity.X *= 0.99f;
                }
            }
            if (projectile.velocity.X > projVec.X)
            {
                projectile.velocity.X -= idleMvt;
                if (projectile.velocity.X > 0f)
                {
                    projectile.velocity.X *= 0.99f;
                }
            }
            if (projectile.velocity.Y < projVec.Y)
            {
                projectile.velocity.Y += idleMvt;
                if (projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y *= 0.99f;
                }
            }
            if (projectile.velocity.Y > projVec.Y)
            {
                projectile.velocity.Y -= idleMvt;
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y *= 0.99f;
                }
            }

            //Rotation if moving
            if (projectile.velocity.X != 0f || projectile.velocity.Y != 0f)
            {
                projectile.rotation = projectile.velocity.X * 0.05f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/BendyPetGlow");
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color rainbow = CalamityUtils.MulticolorLerp(Main.GlobalTime / 2f % 1f, new Color[]
            {
                new Color(255, 0, 0, 50), //Red
                new Color(255, 255, 0, 50), //Yellow
                new Color(0, 255, 0, 50), //Green
                new Color(0, 255, 255, 50), //Cyan
                new Color(0, 0, 255, 50), //Blue
                new Color(255, 0, 255, 50), //Fuschia
            });

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), rainbow, projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
        }
    }
}

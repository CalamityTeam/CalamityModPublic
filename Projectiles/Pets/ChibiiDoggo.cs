using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class ChibiiDoggo : ModProjectile
    {
        public int trueType;
        public bool previousCollide = false;
        public bool yFlip = false; //used to suppress y velocity (pet fastfalls with an extra update per tick otherwise)
        public float notlocalai1 = 0f;
        //used for companion cube AI section; using the actual projectile.ai[1] seems to conflict with black cat AI and cause pet to freeze up sometimes

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibii Devourer");
            Main.projFrames[projectile.type] = 11; //same as black cat
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 46;
            projectile.aiStyle = 26;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.scale = 0.8f;

            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;

            trueType = projectile.type;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
            Texture2D texture2D3 = ModContent.GetTexture("CalamityMod/Projectiles/Pets/ChibiiDoggoMonochrome");
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num156 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;

            int num161 = num159;
            while ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157))
            {
                Color color26 = color25;
                color26 = projectile.GetAlpha(color26);
                goto IL_6899;
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                Vector2 value4 = projectile.oldPos[num161];
                float num165 = projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + projectile.rotation * num160 * (float)(num161 - 1) * projectile.spriteDirection, origin2, projectile.scale, effects, 0f);
                goto IL_6881;
            }
            Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool PreAI()
        {
            projectile.type = 319; //tricks AI 26 into thinking this is a black cat, makes it act like black cat
            Player player = Main.player[projectile.owner];
            player.blackCat = false; //ensure doesnt interact weird with actual black cat
            return true;
        }

        //negates effect of y velocity every other tick
        //pet updates twice per second, so this effectively gives it normal gravity despite otherwise doubled speed
        //has full y speed when jumping up and when tile-intangible (i.e. chasing player when they move out of range)
        public void PreventFastfall()
        {
            yFlip = !yFlip;

            if (yFlip && projectile.velocity.Y > 0)
                projectile.position.Y -= projectile.velocity.Y;
        }

        public void SpawnDoggo()
        {
            Player player = Main.player[projectile.owner];
            string doggoType = "DevourerofGodsHead";

            if (player.name == "Terry")
            {
                doggoType += "S";
                Main.NewText("It's not over yet, kid!", Color.Cyan);
            }
            else
            {
                doggoType += "2";
                Main.NewText("Don't get cocky, kid!", Color.Cyan);
            }

            int type = mod.NPCType(doggoType);

            int n = NPC.NewNPC((int)projectile.position.X, (int)projectile.position.Y, type);
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, (NetworkText)null, n, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            Main.npc[n].netUpdate = true;
        }

        public override void AI()
        {
            projectile.type = trueType; //asserts real type after running black Cat AI 26 (ensures chibii pet isnt drawn as black cat and other weird stuff)

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.chibii = false;
            }
            if (modPlayer.chibii)
            {
                projectile.timeLeft = 2;
            }
            //creates dust whenever switching between flying and grounded like when player spawns/despawns mount
            if (previousCollide != projectile.tileCollide)
            {
                previousCollide = projectile.tileCollide;

                for (int i = 0; i < 10; i++) //resets oldpos so trail doesn't "snap" to chibii from previous position after unhiding
                    projectile.oldPos[i] = projectile.position;

                for (int i = 0; i < 77; i++) //loop to make lots of dust
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 182, projectile.velocity.X * 0.7f, projectile.velocity.Y * 0.7f, 100, default, 2.5f);

                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.5f;
                    Main.dust[dust].noLight = true;

                    int rng = Main.rand.Next(3);

                    if (rng == 0)
                    {
                        Main.dust[dust].velocity *= 1.5f;
                        Main.dust[dust].scale *= 0.7f;
                    }
                    else if (rng == 1)
                    {
                        Main.dust[dust].velocity *= 1.2f;
                        Main.dust[dust].scale *= 0.9f;
                    }
                }
            }

            if (projectile.tileCollide)
            {
                projectile.hide = false;

                //if (projectile.extraUpdates == 1)
                PreventFastfall();

                /*Vector2 distance = Main.player[projectile.owner].Center - projectile.Center;
                if (distance.Length() < 100)
                    projectile.extraUpdates = 0;
                else
                    projectile.extraUpdates = 1;*/
            }
            else
            {
                projectile.hide = true;
                //projectile.extraUpdates = 1;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<ChibiiDoggoFly>()] <= 0)
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, 0f, 0f, ModContent.ProjectileType<ChibiiDoggoFly>(), 0, 0f, projectile.owner, (float)projectile.identity);
            }

            //companion cube lighting check and stab
            Color color;
            color = Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);
            Vector3 vector3_1 = color.ToVector3();
            color = Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16);
            Vector3 vector3_2 = color.ToVector3();

            if ((double)vector3_1.Length() < 0.150000005960464 && (double)vector3_2.Length() < 0.150000005960464)
            {
                notlocalai1 += 1;
            }
            else if ((double)notlocalai1 > 0.0)
            {
                notlocalai1 -= 1;
            }

            notlocalai1 = MathHelper.Clamp(notlocalai1, -3600f, 120f);

            if ((double)notlocalai1 > (double)Main.rand.Next(30, 120) && !player.immune && player.velocity.X == 0 && player.velocity.Y == 0)
            {
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(2))
                    {
                        int style = 5 + Main.rand.Next(5);
                        Main.PlaySound(SoundID.Meowmere, (int)projectile.position.X, (int)projectile.position.Y, style, 4f, 0.0f); //nya
                    }
                    else
                    {
                        Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 2, 2f); //REEEEEEE
                    }
                    notlocalai1 = -600f;
                }
                else
                {
                    notlocalai1 = (float)(Main.rand.Next(30) * -10 - 300);
                    Main.PlaySound(SoundID.Item1, projectile.Center);
                    if (Main.rand.NextBool(2))
                    {
                        player.Hurt(PlayerDeathReason.ByOther(6), 500, 0, false, false, false, -1);
                    }
                    else
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " couldn't stand the sharp objects."), 500, 0, false, false, false, -1);
                    }
                    player.RemoveAllIFrames();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        SpawnDoggo();
                }
                projectile.netUpdate = true;
            }
        }
    }
}

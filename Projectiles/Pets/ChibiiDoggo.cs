using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Main.projFrames[Projectile.type] = 11; //same as black cat
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 38;
            Projectile.height = 46;
            Projectile.aiStyle = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.scale = 0.8f;

            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;

            trueType = Projectile.type;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color color25 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Texture2D texture2D3 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/ChibiiDoggoMonochrome");
            int num156 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y3 = num156 * Projectile.frame;
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
                color26 = Projectile.GetAlpha(color26);
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
                color26 *= num164 / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                Vector2 value4 = Projectile.oldPos[num161];
                float num165 = Projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + Projectile.rotation * num160 * (float)(num161 - 1) * Projectile.spriteDirection, origin2, Projectile.scale, effects, 0f);
                goto IL_6881;
            }
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.position + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreAI()
        {
            Projectile.type = 319; //tricks AI 26 into thinking this is a black cat, makes it act like black cat
            Player player = Main.player[Projectile.owner];
            player.blackCat = false; //ensure doesnt interact weird with actual black cat
            return true;
        }

        //negates effect of y velocity every other tick
        //pet updates twice per second, so this effectively gives it normal gravity despite otherwise doubled speed
        //has full y speed when jumping up and when tile-intangible (i.e. chasing player when they move out of range)
        public void PreventFastfall()
        {
            yFlip = !yFlip;

            if (yFlip && Projectile.velocity.Y > 0)
                Projectile.position.Y -= Projectile.velocity.Y;
        }

        public void SpawnDoggo()
        {
            Player player = Main.player[Projectile.owner];
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

            int type = Mod.Find<ModNPC>(doggoType).Type;

            int n = NPC.NewNPC((int)Projectile.position.X, (int)Projectile.position.Y, type);
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, (NetworkText)null, n, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            Main.npc[n].netUpdate = true;
        }

        public override void AI()
        {
            Projectile.type = trueType; //asserts real type after running black Cat AI 26 (ensures chibii pet isnt drawn as black cat and other weird stuff)

            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.chibii = false;
            }
            if (modPlayer.chibii)
            {
                Projectile.timeLeft = 2;
            }
            //creates dust whenever switching between flying and grounded like when player spawns/despawns mount
            if (previousCollide != Projectile.tileCollide)
            {
                previousCollide = Projectile.tileCollide;

                for (int i = 0; i < 10; i++) //resets oldpos so trail doesn't "snap" to chibii from previous position after unhiding
                    Projectile.oldPos[i] = Projectile.position;

                for (int i = 0; i < 77; i++) //loop to make lots of dust
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 182, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 100, default, 2.5f);

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

            if (Projectile.tileCollide)
            {
                Projectile.hide = false;

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
                Projectile.hide = true;
                //projectile.extraUpdates = 1;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<ChibiiDoggoFly>()] <= 0)
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position.X, Projectile.position.Y, 0f, 0f, ModContent.ProjectileType<ChibiiDoggoFly>(), 0, 0f, Projectile.owner, (float)Projectile.identity);
            }

            //companion cube lighting check and stab
            Color color;
            color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
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
                        SoundEngine.PlaySound(SoundID.Meowmere, (int)Projectile.position.X, (int)Projectile.position.Y, style, 4f, 0.0f); //nya
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 2, 2f); //REEEEEEE
                    }
                    notlocalai1 = -600f;
                }
                else
                {
                    notlocalai1 = (float)(Main.rand.Next(30) * -10 - 300);
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
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
                Projectile.netUpdate = true;
            }
        }
    }
}

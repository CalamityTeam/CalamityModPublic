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
    public class ChibiiDoggo : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public int trueType;
        public bool previousCollide = false;
        public bool yFlip = false; //used to suppress y velocity (pet fastfalls with an extra update per tick otherwise)
        public float notlocalai1 = 0f;
        //used for companion cube AI section; using the actual projectile.ai[1] seems to conflict with black cat AI and cause pet to freeze up sometimes

        public override void SetStaticDefaults()
        {
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
            Projectile.aiStyle = ProjAIStyleID.Pet;
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
            Color colorArea = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Texture2D texture2D3 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/ChibiiDoggoMonochrome").Value;
            int textureArea = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y3 = textureArea * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, textureArea);
            Vector2 halfRect = rectangle.Size() / 2f;
            int eightCompare = 8;
            int twoConst = 2;

            int counter = 1;
            while ((twoConst > 0 && counter < eightCompare) || (twoConst < 0 && counter > eightCompare))
            {
                Color colorAlpha = colorArea;
                colorAlpha = Projectile.GetAlpha(colorAlpha);
                goto IL_6899;
                IL_6881:
                counter += twoConst;
                continue;
                IL_6899:
                float trailColorChange = (float)(eightCompare - counter);
                if (twoConst < 0)
                {
                    trailColorChange = (float)(1 - counter);
                }
                colorAlpha *= trailColorChange / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                Vector2 oldDrawPos = Projectile.oldPos[counter];
                float projRotate = Projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, oldDrawPos + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), colorAlpha, projRotate + Projectile.rotation * 0f * (float)(counter - 1) * Projectile.spriteDirection, halfRect, Projectile.scale, effects, 0f);
                goto IL_6881;
            }
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.position + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, Projectile.rotation, halfRect, Projectile.scale, spriteEffects, 0);
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
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, 0f, 0f, ModContent.ProjectileType<ChibiiDoggoFly>(), 0, 0f, Projectile.owner, (float)Projectile.identity);
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
                    if (Main.rand.NextBool())
                    {
                        SoundEngine.PlaySound(SoundID.Meowmere with { Volume = SoundID.Meowmere.Volume * 4f}, Projectile.position); //nya
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.ScaryScream with { Volume = SoundID.ScaryScream.Volume * 2f}, player.position); //REEEEEEE
                    }
                    notlocalai1 = -600f;
                }
                else
                {
                    notlocalai1 = (float)(Main.rand.Next(30) * -10 - 300);
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                    if (Main.rand.NextBool())
                    {
                        player.Hurt(PlayerDeathReason.ByOther(6), 500, 0);
                    }
                    else
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ChibiiDoggo").Format(player.name)), 500, 0);
                    }
                    player.RemoveAllIFrames();
                }
                Projectile.netUpdate = true;
            }
        }
    }
}

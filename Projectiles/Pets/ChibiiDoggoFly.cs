using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class ChibiiDoggoFly : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 24;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.scale = 0.8f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color colorArea = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Texture2D texture2D3 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/ChibiiDoggoFlyMonochrome").Value;
            int textureArea = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y3 = textureArea * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, textureArea);
            Vector2 origin2 = rectangle.Size() / 2f;
            int twelveCompare = 12;
            int twoConst = 2;
            int counter = 1;
            while ((twoConst > 0 && counter < twelveCompare) || (twoConst < 0 && counter > twelveCompare))
            {
                Color colorAlpha = colorArea;
                colorAlpha = Projectile.GetAlpha(colorAlpha);
                goto IL_6899;
                IL_6881:
                counter += twoConst;
                continue;
                IL_6899:
                float trailColorChange = (float)(twelveCompare - counter);
                if (twoConst < 0)
                {
                    trailColorChange = (float)(1 - counter);
                }
                colorAlpha *= trailColorChange / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                Vector2 oldDrawPos = Projectile.oldPos[counter];
                float projRotate = Projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, oldDrawPos + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), colorAlpha, projRotate + Projectile.rotation * 0f * (float)(counter - 1) * Projectile.spriteDirection, origin2, Projectile.scale, effects, 0f);
                goto IL_6881;
            }
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.position + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override bool PreAI()
        {
            //parent projectile might spawn more than one ChibiiDoggoFly sometimes
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ChibiiDoggoFly>()] > 1)
            {
                Projectile.hide = true;
                Projectile.Kill();
                Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ChibiiDoggoFly>()]--;
                return false;
            }

            return true;
        }

        public override void AI()
        {
            if (Main.player[Projectile.owner].Calamity().chibii)
                Projectile.timeLeft = 2;

            int byUuid = (int)Projectile.ai[0]; //contains identity of parent projectile in main.projectile array

            //basically slaved to parent's position
            //this projectile only exists because i fucking hate that tile collision desync i got when using full-sized flying sprites in ChibiiDoggo
            if (Main.projectile[byUuid].active && Main.projectile[byUuid].type == ModContent.ProjectileType<ChibiiDoggo>())
            {
                //projectile.position.X = Main.projectile[byUuid].position.X - 22;
                //projectile.position.Y = Main.projectile[byUuid].position.Y;
                Projectile.Center = Main.projectile[byUuid].Center;
                Projectile.rotation = Main.projectile[byUuid].rotation;
                Projectile.spriteDirection = Main.projectile[byUuid].spriteDirection;

                if (Main.projectile[byUuid].tileCollide)
                    Projectile.hide = true;
                else
                    Projectile.hide = false;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class ChibiiDoggoFly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibii Devourer (Flying)");
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 24;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.scale = 0.8f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
            Texture2D texture2D3 = ModContent.GetTexture("CalamityMod/Projectiles/Pets/ChibiiDoggoFlyMonochrome");
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num156 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 12;
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
            //parent projectile might spawn more than one ChibiiDoggoFly sometimes
            if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ChibiiDoggoFly>()] > 1)
            {
                projectile.hide = true;
                projectile.Kill();
                Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ChibiiDoggoFly>()]--;
                return false;
            }

            return true;
        }

        public override void AI()
        {
            if (Main.player[projectile.owner].Calamity().chibii)
                projectile.timeLeft = 2;

            int byUuid = (int)projectile.ai[0]; //contains identity of parent projectile in main.projectile array

            //basically slaved to parent's position
            //this projectile only exists because i fucking hate that tile collision desync i got when using full-sized flying sprites in ChibiiDoggo
            if (Main.projectile[byUuid].active && Main.projectile[byUuid].type == ModContent.ProjectileType<ChibiiDoggo>())
            {
                //projectile.position.X = Main.projectile[byUuid].position.X - 22;
                //projectile.position.Y = Main.projectile[byUuid].position.Y;
                projectile.Center = Main.projectile[byUuid].Center;
                projectile.rotation = Main.projectile[byUuid].rotation;
                projectile.spriteDirection = Main.projectile[byUuid].spriteDirection;

                if (Main.projectile[byUuid].tileCollide)
                    projectile.hide = true;
                else
                    projectile.hide = false;
            }
        }
    }
}

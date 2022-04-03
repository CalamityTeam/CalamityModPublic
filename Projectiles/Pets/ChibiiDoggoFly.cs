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
            Color color25 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Texture2D texture2D3 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/ChibiiDoggoFlyMonochrome");
            int num156 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y3 = num156 * Projectile.frame;
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

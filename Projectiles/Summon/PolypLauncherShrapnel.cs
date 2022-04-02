using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class PolypLauncherShrapnel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polyp Shrapnel");
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 13;
            projectile.height = 13;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
            projectile.minion = true;
            projectile.minionSlots = 0f;
        }

        public override void AI()
        {
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y += 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 225, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 150, default, 0.7f);
                splash += 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (projectile.ai[0] == 1f)
            {
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/PolypLauncherShrapnel2");
            }
            if (projectile.ai[0] == 2f)
            {
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/PolypLauncherShrapnel3");
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, texture.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

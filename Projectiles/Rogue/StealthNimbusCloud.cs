using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class StealthNimbusCloud : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/ShadeNimbusCloud";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nimbus");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 25;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (projectile.ai[0] == 1f)
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/StealthNimbusCloud2");
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.02f;
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                {
                    projectile.frame = 0;
                    return;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<StealthNimbus>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], 0f);
            }
        }

        public override bool CanDamage() => false;
    }
}

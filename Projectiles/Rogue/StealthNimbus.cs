using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class StealthNimbus : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/ShadeNimbusHostile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nimbus");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 24;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (projectile.ai[0] == 1f)
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/StealthNimbus2");
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
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 5)
                {
                    projectile.frame = 0;
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 3600f)
            {
                projectile.alpha += 5;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }
            else
            {
                if (projectile.timeLeft % 8 == 0)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        int num414 = (int)(projectile.position.X + 14f + (float)Main.rand.Next(projectile.width - 28));
                        int num415 = (int)(projectile.position.Y + (float)projectile.height + 4f);
                        Projectile.NewProjectile((float)num414, (float)num415, 0f, 5f, ModContent.ProjectileType<StealthRain>(), projectile.damage, 0f, projectile.owner, projectile.ai[0], 0f);
                    }
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 10f)
            {
                projectile.localAI[0] = 0f;
                int projCount = 0;
                int oldestCloud = 0;
                float cloudAge = 0f;
                int projType = projectile.type;
                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    if (proj.active && proj.owner == projectile.owner && proj.type == projType && proj.ai[1] < 3600f)
                    {
                        projCount++;
                        if (proj.ai[1] > cloudAge)
                        {
                            oldestCloud = projIndex;
                            cloudAge = proj.ai[1];
                        }
                    }
                }
                if (projCount > 2)
                {
                    Main.projectile[oldestCloud].netUpdate = true;
                    Main.projectile[oldestCloud].ai[1] = 36000f;
                    return;
                }
            }
        }

        public override bool CanDamage() => false;
    }
}

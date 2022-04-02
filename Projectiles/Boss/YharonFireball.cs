using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class YharonFireball : ModProjectile
    {
        private float speedX = -3f;
        private float speedX2 = -5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Fireball");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 34;
            projectile.height = 34;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
            projectile.aiStyle = 1;
            aiType = ProjectileID.DD2BetsyFireball;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.rotation += MathHelper.Pi;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedX);
            writer.Write(projectile.localAI[1]);
            writer.Write(speedX2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedX = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            speedX2 = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                    Color color2 = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.rotation, rectangle.Size() / 2f, projectile.scale, spriteEffects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, texture.Width, frameHeight)),
                projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int x = 0; x < 3; x++)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speedX, -50f, ModContent.ProjectileType<YharonFireball2>(), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                    speedX += 3f;
                }
                for (int x = 0; x < 2; x++)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speedX2, -75f, ModContent.ProjectileType<YharonFireball2>(), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                    speedX2 += 10f;
                }
            }
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 144);
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 50, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 50, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}

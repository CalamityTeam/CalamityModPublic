using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class Flare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

            float num630 = 0.209439516f;
            float num631 = 4f;
            float num632 = (float)(Math.Cos((double)(num630 * projectile.ai[0])) - 0.5) * num631;
            projectile.velocity.Y = projectile.velocity.Y - num632;
            projectile.ai[0] += 1f;
            num632 = (float)(Math.Cos((double)(num630 * projectile.ai[0])) - 0.5) * num631;
            projectile.velocity.Y = projectile.velocity.Y + num632;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 10f)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }

            if (projectile.wet)
            {
                projectile.position.Y = projectile.position.Y - 16f;
                projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            Main.PlaySound(SoundID.Item20, projectile.position);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 244, vector7.X * 2f, vector7.Y * 2f, 100, default, 1.4f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            if (projectile.owner == Main.myPlayer)
            {
                int num231 = (int)(projectile.Center.Y / 16f);
                int num232 = (int)(projectile.Center.X / 16f);
                int num233 = 100;
                if (num232 < 10)
                {
                    num232 = 10;
                }
                if (num232 > Main.maxTilesX - 10)
                {
                    num232 = Main.maxTilesX - 10;
                }
                if (num231 < 10)
                {
                    num231 = 10;
                }
                if (num231 > Main.maxTilesY - num233 - 10)
                {
                    num231 = Main.maxTilesY - num233 - 10;
                }
                for (int num234 = num231; num234 < num231 + num233; num234++)
                {
                    Tile tile = Main.tile[num232, num234];
                    if (tile.active() && (Main.tileSolid[(int)tile.type] || tile.liquid != 0))
                    {
                        num231 = num234;
                        break;
                    }
                }
                int spawnLimitY = (int)(Main.player[projectile.owner].Center.Y / 16f) + 25;
                if (num231 > spawnLimitY)
                {
                    num231 = spawnLimitY;
                }
                int num236 = Projectile.NewProjectile((float)(num232 * 16 + 8), (float)(num231 * 16 - 24), 0f, 0f, ModContent.ProjectileType<Flarenado>(), 0, 4f, Main.myPlayer, 11f, 10f + (revenge ? 1f : 0f));
                Main.projectile[num236].netUpdate = true;
            }
        }
    }
}

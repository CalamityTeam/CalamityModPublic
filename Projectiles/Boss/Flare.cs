using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class Flare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            float num630 = 0.209439516f;
            float num631 = 4f;
            float num632 = (float)(Math.Cos((double)(num630 * Projectile.ai[0])) - 0.5) * num631;
            Projectile.velocity.Y = Projectile.velocity.Y - num632;
            Projectile.ai[0] += 1f;
            num632 = (float)(Math.Cos((double)(num630 * Projectile.ai[0])) - 0.5) * num631;
            Projectile.velocity.Y = Projectile.velocity.Y + num632;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 10f)
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 100)
                {
                    Projectile.alpha = 100;
                }
            }

            if (Projectile.wet)
            {
                Projectile.position.Y = Projectile.position.Y - 16f;
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 244, vector7.X * 2f, vector7.Y * 2f, 100, default, 1.4f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int num231 = (int)(Projectile.Center.Y / 16f);
                int num232 = (int)(Projectile.Center.X / 16f);
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
                    if (tile.HasTile && (Main.tileSolid[(int)tile.TileType] || tile.liquid != 0))
                    {
                        num231 = num234;
                        break;
                    }
                }
                int spawnLimitY = (int)(Main.player[Projectile.owner].Center.Y / 16f) + 25;
                if (num231 > spawnLimitY)
                {
                    num231 = spawnLimitY;
                }
                int num236 = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), (float)(num232 * 16 + 8), (float)(num231 * 16 - 24), 0f, 0f, ModContent.ProjectileType<Flarenado>(), 0, 4f, Main.myPlayer, 11f, 10f + (revenge ? 1f : 0f));
                Main.projectile[num236].netUpdate = true;
            }
        }
    }
}

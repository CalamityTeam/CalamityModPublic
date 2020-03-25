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
    public class BirbAuraFlare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Birb Aura Flare");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.hostile = true;
			projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
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
            if (projectile.ai[1] > 0f)
            {
                int num625 = (int)projectile.ai[1] - 1;
                if (num625 < 255)
                {
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] > 10f)
                    {
						// Dust pulse effect
						projectile.ai[0] = (float)Math.Abs(Math.Cos((double)MathHelper.ToRadians(projectile.localAI[0] * 2f)));
                        int num626 = 18;
                        for (int num627 = 0; num627 < num626; num627++)
                        {
                            Vector2 vector45 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * projectile.ai[0];
                            vector45 = vector45.RotatedBy((double)(num627 - (num626 / 2 - 1)) * 3.1415926535897931 / (double)(float)num626, default) + projectile.Center;
                            Vector2 value15 = ((float)(Main.rand.NextDouble() * Math.PI) - MathHelper.PiOver2).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int num628 = Dust.NewDust(vector45 + value15, 0, 0, 60, value15.X * 2f, value15.Y * 2f, 100, default, 1f);
							Main.dust[num628].scale = 1.4f;
                            Main.dust[num628].noGravity = true;
                            Main.dust[num628].noLight = true;
                            Main.dust[num628].velocity /= 4f;
                            Main.dust[num628].velocity -= projectile.velocity;
                        }
					}

                    Vector2 value16 = Main.player[num625].Center - projectile.Center;
                    float num629 = 4f;
                    num629 += projectile.localAI[0] / 60f;
                    projectile.velocity = Vector2.Normalize(value16) * num629;
                    if (value16.Length() < 32f)
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			Main.PlaySound(SoundID.DD2_BetsyFireballImpact, (int)projectile.position.X, (int)projectile.position.Y);
			int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 60, vector7.X, vector7.Y, 100, default, 1.4f);
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
				int spawnAreaY = Main.maxTilesY - num231;
				for (int num234 = num231; num234 < num231 + spawnAreaY; num234++)
				{
					Tile tile = Main.tile[num232, num234];
					if (tile.active() && !TileID.Sets.Platforms[tile.type] && (Main.tileSolid[(int)tile.type] || tile.liquid != 0))
					{
						num231 = num234;
						break;
					}
				}
				float x = (float)(num232 * 16);
				float y = (float)(num231 * 16 - 48);
				Vector2 laserVelocity = new Vector2(x, 160f) - new Vector2(x, y);
				laserVelocity.Normalize();
				int damage = Main.expertMode ? 75 : 100;
				int num236 = Projectile.NewProjectile(x, y, 0f, laserVelocity.Y, ModContent.ProjectileType<BirbAura>(), damage, 0f, Main.myPlayer, x, y);
                Main.projectile[num236].netUpdate = true;
            }
        }
    }
}

using CalamityMod.NPCs.Bumblebirb;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class BirbAuraFlare : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Aura Flare");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0f)
            {
                int num625 = (int)Projectile.ai[1] - 1;
                if (num625 < 255)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 10f)
                    {
                        // Dust pulse effect
                        Projectile.localAI[1] = (float)Math.Abs(Math.Cos(MathHelper.ToRadians(Projectile.localAI[0] * 2f)));
                        int num626 = 18;
                        for (int num627 = 0; num627 < num626; num627++)
                        {
                            Vector2 vector45 = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * Projectile.localAI[1];
                            vector45 = vector45.RotatedBy((num627 - (num626 / 2 - 1)) * 3.1415926535897931 / (float)num626) + Projectile.Center;
                            Vector2 value15 = ((float)(Main.rand.NextDouble() * Math.PI) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                            int num628 = Dust.NewDust(vector45 + value15, 0, 0, 60, value15.X * 2f, value15.Y * 2f, 100, default, 1f);
                            Main.dust[num628].scale = 1.4f;
                            Main.dust[num628].noGravity = true;
                            Main.dust[num628].noLight = true;
                            Main.dust[num628].velocity /= 4f;
                            Main.dust[num628].velocity -= Projectile.velocity;
                        }
                    }

                    Vector2 value16 = Main.player[num625].Center - Projectile.Center;
                    float num629 = 4f;
                    float divisor = 60f - 15f * Projectile.ai[0];
                    num629 += Projectile.localAI[0] / divisor;
                    Projectile.velocity = Vector2.Normalize(value16) * num629;
                    if (value16.Length() < 32f)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, (int)Projectile.position.X, (int)Projectile.position.Y);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / num226) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 60, vector7.X, vector7.Y, 100, default, 1.4f);
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
                float x = num232 * 16;
                float y = num231 * 16 + 900;
                Vector2 laserVelocity = new Vector2(x, 160f) - new Vector2(x, y);
                int type = ModContent.ProjectileType<BirbAura>();
                int damage = Projectile.GetProjectileDamage(ModContent.NPCType<Bumblefuck>());
                if (Projectile.ai[0] >= 2f)
                {
                    x += 1000f;
                    if ((int)(x / 16f) > Main.maxTilesX - 10)
                    {
                        x = (Main.maxTilesX - 10) * 16f;
                    }
                    laserVelocity = new Vector2(x, 160f) - new Vector2(x, y);
                    laserVelocity.Normalize();
                    int num237 = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), x, y, 0f, laserVelocity.Y, type, damage, 0f, Main.myPlayer, x, y);
                    Main.projectile[num237].timeLeft = 900;
                    Main.projectile[num237].netUpdate = true;

                    x -= 2000f;
                    if ((int)(x / 16f) < 10)
                    {
                        x = 160f;
                    }
                    laserVelocity = new Vector2(x, 160f) - new Vector2(x, y);
                    laserVelocity.Normalize();
                    int num238 = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), x, y, 0f, laserVelocity.Y, type, damage, 0f, Main.myPlayer, x, y);
                    Main.projectile[num238].timeLeft = 900;
                    Main.projectile[num238].netUpdate = true;
                }
                else
                {
                    laserVelocity.Normalize();
                    int num236 = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), x, y, 0f, laserVelocity.Y, type, damage, 0f, Main.myPlayer, x, y);
                    Main.projectile[num236].netUpdate = true;
                }
            }
        }
    }
}

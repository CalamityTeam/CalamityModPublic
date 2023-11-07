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
    public class BirbAuraFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

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
                int playerTracker = (int)Projectile.ai[1] - 1;
                if (playerTracker < 255)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 10f)
                    {
                        // Dust pulse effect
                        Projectile.localAI[1] = (float)Math.Abs(Math.Cos(MathHelper.ToRadians(Projectile.localAI[0] * 2f)));
                        int dustAmt = 18;
                        for (int i = 0; i < dustAmt; i++)
                        {
                            Vector2 dustRotation = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * Projectile.localAI[1];
                            dustRotation = dustRotation.RotatedBy((i - (dustAmt / 2 - 1)) * 3.1415926535897931 / (float)dustAmt) + Projectile.Center;
                            Vector2 randomDustPos = ((float)(Main.rand.NextDouble() * Math.PI) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                            int lightningDust = Dust.NewDust(dustRotation + randomDustPos, 0, 0, 60, randomDustPos.X * 2f, randomDustPos.Y * 2f, 100, default, 1f);
                            Main.dust[lightningDust].scale = 1.4f;
                            Main.dust[lightningDust].noGravity = true;
                            Main.dust[lightningDust].noLight = true;
                            Main.dust[lightningDust].velocity /= 4f;
                            Main.dust[lightningDust].velocity -= Projectile.velocity;
                        }
                    }

                    Vector2 playerDistance = Main.player[playerTracker].Center - Projectile.Center;
                    float projVelocityMult = 4f;
                    float divisor = 60f - 15f * Projectile.ai[0];
                    projVelocityMult += Projectile.localAI[0] / divisor;
                    Projectile.velocity = Vector2.Normalize(playerDistance) * projVelocityMult;
                    if (playerDistance.Length() < 32f)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
            int killDustAmt = 36;
            for (int i = 0; i < killDustAmt; i++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((i - (killDustAmt / 2 - 1)) * MathHelper.TwoPi / killDustAmt) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int killLightningDust = Dust.NewDust(vector6 + vector7, 0, 0, 60, vector7.X, vector7.Y, 100, default, 1.4f);
                Main.dust[killLightningDust].noGravity = true;
                Main.dust[killLightningDust].noLight = true;
                Main.dust[killLightningDust].velocity = vector7;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int projTileX = (int)(Projectile.Center.Y / 16f);
                int projTileY = (int)(Projectile.Center.X / 16f);
                if (projTileY < 10)
                {
                    projTileY = 10;
                }
                if (projTileY > Main.maxTilesX - 10)
                {
                    projTileY = Main.maxTilesX - 10;
                }
                if (projTileX < 10)
                {
                    projTileX = 10;
                }
                if (projTileX > Main.maxTilesY - 110)
                {
                    projTileX = Main.maxTilesY - 110;
                }
                float x = projTileY * 16;
                float y = projTileX * 16 + 900;
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
                    int thirdPhaseRightLaser = Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, 0f, laserVelocity.Y, type, damage, 0f, Main.myPlayer, x, y);
                    Main.projectile[thirdPhaseRightLaser].timeLeft = 900;
                    Main.projectile[thirdPhaseRightLaser].netUpdate = true;

                    x -= 2000f;
                    if ((int)(x / 16f) < 10)
                    {
                        x = 160f;
                    }
                    laserVelocity = new Vector2(x, 160f) - new Vector2(x, y);
                    laserVelocity.Normalize();
                    int thirdPhaseLeftLaser = Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, 0f, laserVelocity.Y, type, damage, 0f, Main.myPlayer, x, y);
                    Main.projectile[thirdPhaseLeftLaser].timeLeft = 900;
                    Main.projectile[thirdPhaseLeftLaser].netUpdate = true;
                }
                else
                {
                    laserVelocity.Normalize();
                    int secondPhaseLaser = Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, 0f, laserVelocity.Y, type, damage, 0f, Main.myPlayer, x, y);
                    Main.projectile[secondPhaseLaser].netUpdate = true;
                }
            }
        }
    }
}

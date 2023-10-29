using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class TyphoonBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Melee/BrinyTyphoonBubble";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0f)
            {
                int playerOwner = (int)Projectile.ai[1] - 1;
                if (playerOwner < 255)
                {
                    Projectile.localAI[0] += 1f;
                    if (Projectile.localAI[0] > 10f)
                    {
                        int six = 6;
                        for (int i = 0; i < six; i++)
                        {
                            Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                            rotate = rotate.RotatedBy((double)(i - (six / 2 - 1)) * 3.1415926535897931 / (double)(float)six, default) + Projectile.Center;
                            Vector2 dustPos = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int typhoonDust = Dust.NewDust(rotate + dustPos, 0, 0, 172, dustPos.X * 2f, dustPos.Y * 2f, 100, default, 1.4f);
                            Main.dust[typhoonDust].noGravity = true;
                            Main.dust[typhoonDust].noLight = true;
                            Main.dust[typhoonDust].velocity /= 4f;
                            Main.dust[typhoonDust].velocity -= Projectile.velocity;
                        }
                        Projectile.alpha -= 5;
                        if (Projectile.alpha < 100)
                        {
                            Projectile.alpha = 100;
                        }
                        Projectile.rotation += Projectile.velocity.X * 0.1f;
                        Projectile.frame = (int)(Projectile.localAI[0] / 3f) % 3;
                    }
                    Vector2 playerDist = Main.player[playerOwner].Center - Projectile.Center;
                    float velocityMult = 4f;
                    velocityMult += Projectile.localAI[0] / 20f;
                    Projectile.velocity = Vector2.Normalize(playerDist) * velocityMult;
                    if (playerDist.Length() < 50f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                float swaySize = 0.209439516f;
                float smolWidth = 4f;
                float projXChange = (float)(Math.Cos((double)(swaySize * Projectile.ai[0])) - 0.5) * smolWidth;
                Projectile.velocity.Y = Projectile.velocity.Y - projXChange;
                Projectile.ai[0] += 1f;
                projXChange = (float)(Math.Cos((double)(swaySize * Projectile.ai[0])) - 0.5) * smolWidth;
                Projectile.velocity.Y = Projectile.velocity.Y + projXChange;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 10f)
                {
                    Projectile.alpha -= 5;
                    if (Projectile.alpha < 100)
                    {
                        Projectile.alpha = 100;
                    }
                    Projectile.rotation += Projectile.velocity.X * 0.1f;
                    Projectile.frame = (int)(Projectile.localAI[0] / 3f) % 3;
                }
            }
            if (Projectile.wet)
            {
                Projectile.position.Y = Projectile.position.Y - 16f;
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            int constant = 36;
            for (int j = 0; j < constant; j++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(j - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int waterDust = Dust.NewDust(rotate + faceDirection, 0, 0, 172, faceDirection.X * 2f, faceDirection.Y * 2f, 100, default, 1.4f);
                Main.dust[waterDust].noGravity = true;
                Main.dust[waterDust].noLight = true;
                Main.dust[waterDust].velocity = faceDirection;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[1] < 1f)
                {
                    if (Projectile.localAI[1] == 1f)
                    {
                        int nextSegment = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - (float)(Projectile.direction * 30), Projectile.Center.Y - 4f, (float)-(float)Projectile.direction * 0.01f, 0f, ModContent.ProjectileType<SeasSearingSpout>(), Projectile.damage, 3f, Projectile.owner, 16f, 8f);
                        Main.projectile[nextSegment].netUpdate = true;
                    }
                    else
                    {
                        int nextSegment = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - (float)(Projectile.direction * 30), Projectile.Center.Y - 4f, (float)-(float)Projectile.direction * 0.01f, 0f, ModContent.ProjectileType<WaterSpout>(), Projectile.damage, 3f, Projectile.owner, 16f, 8f);
                        Main.projectile[nextSegment].netUpdate = true;
                    }
                }
            }
        }
    }
}

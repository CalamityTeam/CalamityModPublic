using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SepticSkewerBacteria : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            int dustType = 171;
            if (Main.rand.Next(3) == 0)
            {
                dustType = 46;
            }
            int toxicDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
            Main.dust[toxicDust].noGravity = true;
            float scaleAlpha = 1f - (float)Projectile.alpha / 255f;
            scaleAlpha *= Projectile.scale;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 90f)
            {
                Projectile.localAI[0] *= -1f;
            }
            if (Projectile.localAI[0] >= 0f)
            {
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.scale -= 0.003f;
            }
            Projectile.rotation += 0.0025f * Projectile.scale;
            float yVelControl = 1f;
            float xVelControl = 1f;
            if (Projectile.identity % 6 == 0)
            {
                xVelControl *= -1f;
            }
            if (Projectile.identity % 6 == 1)
            {
                yVelControl *= -1f;
            }
            if (Projectile.identity % 6 == 2)
            {
                xVelControl *= -1f;
                yVelControl *= -1f;
            }
            if (Projectile.identity % 6 == 3)
            {
                xVelControl = 0f;
            }
            if (Projectile.identity % 6 == 4)
            {
                yVelControl = 0f;
            }
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 60f)
            {
                Projectile.localAI[1] = -180f;
            }
            if (Projectile.localAI[1] >= -60f)
            {
                Projectile.velocity.X = Projectile.velocity.X + 0.002f * xVelControl;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.002f * yVelControl;
            }
            else
            {
                Projectile.velocity.X = Projectile.velocity.X - 0.002f * xVelControl;
                Projectile.velocity.Y = Projectile.velocity.Y - 0.002f * yVelControl;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5400f)
            {
                Projectile.damage = 0;
                Projectile.ai[1] = 1f;
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 5;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                    }
                }
                else if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                float playerDist = (Projectile.Center - Main.player[Projectile.owner].Center).Length() / 100f;
                if (playerDist > 4f)
                {
                    playerDist *= 1.1f;
                }
                if (playerDist > 5f)
                {
                    playerDist *= 1.2f;
                }
                if (playerDist > 6f)
                {
                    playerDist *= 1.3f;
                }
                if (playerDist > 7f)
                {
                    playerDist *= 1.4f;
                }
                if (playerDist > 8f)
                {
                    playerDist *= 1.5f;
                }
                if (playerDist > 9f)
                {
                    playerDist *= 1.6f;
                }
                if (playerDist > 10f)
                {
                    playerDist *= 1.7f;
                }
                Projectile.ai[0] += playerDist;
                if (Projectile.alpha > 50)
                {
                    Projectile.alpha -= 10;
                    if (Projectile.alpha < 50)
                    {
                        Projectile.alpha = 50;
                    }
                }
            }
            if ((double)Projectile.velocity.Length() > 0.2)
            {
                Projectile.velocity *= 0.98f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 120);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 56;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int constant = 36;
            for (int i = 0; i < constant; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int dust = Dust.NewDust(rotate + faceDirection, 0, 0, 171, faceDirection.X, faceDirection.Y, 100, default, 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = faceDirection;
            }
        }
    }
}

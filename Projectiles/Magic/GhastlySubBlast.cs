using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GhastlySubBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 420;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            bool unusedBool = false; // yeah idk -CIT
            int projType = ModContent.ProjectileType<GhastlyBlast>();
            float x3 = 0.15f;
            float y3 = 0.15f;
            if (unusedBool)
            {
                int projID = (int)Projectile.ai[1];
                if (!Main.projectile[projID].active || Main.projectile[projID].type != projType)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.timeLeft = 2;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 420f)
            {
                bool isActive = true;
                int preHomingProjID = (int)Projectile.ai[1];
                if (Main.projectile[preHomingProjID].active && Main.projectile[preHomingProjID].type == projType)
                {
                    if (Main.projectile[preHomingProjID].oldPos[1] != Vector2.Zero)
                    {
                        Projectile.position += Main.projectile[preHomingProjID].position - Main.projectile[preHomingProjID].oldPos[1];
                    }
                    if (Projectile.Center.HasNaNs())
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                else
                {
                    Projectile.ai[0] = 420f;
                    isActive = false;
                    Projectile.Kill();
                }
                if (isActive)
                {
                    Projectile.velocity += new Vector2((float)Math.Sign(Main.projectile[preHomingProjID].Center.X - Projectile.Center.X), (float)Math.Sign(Main.projectile[preHomingProjID].Center.Y - Projectile.Center.Y)) * new Vector2(x3, y3);
                    if (Projectile.velocity.Length() > 6f)
                    {
                        Projectile.velocity *= 6f / Projectile.velocity.Length();
                    }
                }
                if (Main.rand.NextBool())
                {
                    int ghostlyRed = Dust.NewDust(Projectile.Center, 8, 8, 60, 0f, 0f, 0, default, 1f);
                    Main.dust[ghostlyRed].position = Projectile.Center;
                    Main.dust[ghostlyRed].velocity = Projectile.velocity;
                    Main.dust[ghostlyRed].noGravity = true;
                    Main.dust[ghostlyRed].scale = 1.5f;
                    if (isActive)
                    {
                        Main.dust[ghostlyRed].customData = Main.projectile[(int)Projectile.ai[1]];
                    }
                }
                Projectile.alpha = 255;
                return;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ai[0] = 60f;
            for (int i = 0; i < 10; i++)
            {
                int killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[killDust].scale = 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[killDust].noGravity = true;
                dust = Main.dust[killDust];
                dust.velocity *= 1.25f;
                dust = Main.dust[killDust];
                dust.velocity -= Projectile.oldVelocity / 10f;
            }
        }
    }
}

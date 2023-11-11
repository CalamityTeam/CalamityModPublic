using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulaCloud : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 116;
            Projectile.height = 116;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 720;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            int mainProjType = ModContent.ProjectileType<NebulaCloudCore>();
            float projTimer = 720f;
            bool unusedFlag = false;

            if (unusedFlag)
            {
                int mainProj = (int)Projectile.ai[1];
                if (!Main.projectile[mainProj].active || Main.projectile[mainProj].type != mainProjType)
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.timeLeft = 2;
            }

            Projectile.ai[0]++;
            if (!(Projectile.ai[0] < projTimer))
                return;

            bool isActive = true;
            int mainProjID = (int)Projectile.ai[1];
            if (Main.projectile[mainProjID].active && Main.projectile[mainProjID].type == mainProjType)
            {
                if (Main.projectile[mainProjID].oldPos[1] != Vector2.Zero)
                    Projectile.position += Main.projectile[mainProjID].position - Main.projectile[mainProjID].oldPos[1];

                if (Projectile.Center.HasNaNs())
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile.ai[0] = projTimer;
                isActive = false;
                Projectile.Kill();
            }

            if (isActive)
            {
                Projectile.velocity += new Vector2(Math.Sign(Main.projectile[mainProjID].Center.X - Projectile.Center.X), Math.Sign(Main.projectile[mainProjID].Center.Y - Projectile.Center.Y)) * new Vector2(0.15f, 0.15f);
                if (Projectile.velocity.Length() > 6f)
                    Projectile.velocity *= 6f / Projectile.velocity.Length();
            }

            if (Main.rand.NextBool())
            {
                int purpleDust = Dust.NewDust(Projectile.Center, 8, 8, 86);
                Main.dust[purpleDust].position = Projectile.Center;
                Main.dust[purpleDust].velocity = Projectile.velocity;
                Main.dust[purpleDust].noGravity = true;
                Main.dust[purpleDust].scale = 1.75f;

                if (isActive)
                    Main.dust[purpleDust].customData = Main.projectile[(int)Projectile.ai[1]];
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ai[0] = 86f;

            for (int i = 0; i < 15; i++)
            {
                int killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default(Color), 0.75f);
                Dust dust;
                if (Main.rand.NextBool(3))
                {
                    Main.dust[killDust].fadeIn = 0.75f + Main.rand.Next(-10, 11) * 0.015f;
                    Main.dust[killDust].scale = 0.325f + Main.rand.Next(-10, 11) * 0.0075f;
                    dust = Main.dust[killDust];
                    dust.type++;
                }
                else
                    Main.dust[killDust].scale = 1.5f + Main.rand.Next(-10, 11) * 0.015f;

                Main.dust[killDust].noGravity = true;
                dust = Main.dust[killDust];
                dust.velocity *= 1.375f;
                dust = Main.dust[killDust];
                dust.velocity -= Projectile.oldVelocity / 20f;
            }
        }
    }
}

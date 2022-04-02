using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismaticWave : ModProjectile
    {
        private int alpha = 50;
        public Color[] colors = new Color[]
        {
            new Color(255, 0, 0, 50), //Red
            new Color(255, 128, 0, 50), //Orange
            new Color(255, 255, 0, 50), //Yellow
            new Color(128, 255, 0, 50), //Lime
            new Color(0, 255, 0, 50), //Green
            new Color(0, 255, 128, 50), //Turquoise
            new Color(0, 255, 255, 50), //Cyan
            new Color(0, 128, 255, 50), //Light Blue
            new Color(0, 0, 255, 50), //Blue
            new Color(128, 0, 255, 50), //Purple
            new Color(255, 0, 255, 50), //Fuschia
            new Color(255, 0, 128, 50) //Hot Pink
        };
        List<Color> colorSet = new List<Color>()
        {
            new Color(255, 0, 0, 50), //Red
            new Color(255, 255, 0, 50), //Yellow
            new Color(0, 255, 0, 50), //Green
            new Color(0, 255, 255, 50), //Cyan
            new Color(0, 0, 255, 50), //Blue
            new Color(255, 0, 255, 50), //Fuschia
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
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
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.02f;
                projectile.alpha += 30;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.02f;
                projectile.alpha -= 30;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            if (Main.rand.NextBool(2))
            {
                int rainbow = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 267, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, alpha, Main.rand.Next(colors));
                Main.dust[rainbow].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 355)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            int colorIndex = (int)(Main.GlobalTime / 2 % colorSet.Count);
            Color currentColor = colorSet[colorIndex];
            Color nextColor = colorSet[(colorIndex + 1) % colorSet.Count];
            return Color.Lerp(currentColor, nextColor, Main.GlobalTime % 2f > 1f ? 1f : Main.GlobalTime % 1f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int rainbow = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 267, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, alpha, Main.rand.Next(colors));
                Main.dust[rainbow].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 150);
            target.AddBuff(BuffID.Daybreak, 150);
            projectile.velocity *= 0.75f;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 150);
            target.AddBuff(BuffID.Daybreak, 150);
            projectile.velocity *= 0.75f;
        }
    }
}

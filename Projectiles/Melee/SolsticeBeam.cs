using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SolsticeBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.5f);
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item60, projectile.position);
            }
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
            int dustType = 0;

            switch (CalamityMod.CurrentSeason)
            {
                case Season.Spring:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        74,
                        157,
                        107
                    });
                    break;
                case Season.Summer:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        247,
                        228,
                        57
                    });
                    break;
                case Season.Fall:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        6,
                        259,
                        158
                    });
                    break;
                case Season.Winter:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        67,
                        229,
                        185
                    });
                    break;
            }
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            byte red = 255;
            byte green = 255;
            byte blue = 255;
            switch (CalamityMod.CurrentSeason)
            {
                case Season.Spring:
                    red = 0;
                    green = 250;
                    blue = 0;
                    break;
                case Season.Summer:
                    red = 250;
                    green = 250;
                    blue = 0;
                    break;
                case Season.Fall:
                    red = 250;
                    green = 150;
                    blue = 50;
                    break;
                case Season.Winter:
                    red = 100;
                    green = 150;
                    blue = 250;
                    break;
            }
            return new Color(red, green, blue, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int dustType = 0;

            switch (CalamityMod.CurrentSeason)
            {
                case Season.Spring:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        245,
                        157,
                        107
                    });
                    break;
                case Season.Summer:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        247,
                        228,
                        57
                    });
                    break;
                case Season.Fall:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        6,
                        259,
                        158
                    });
                    break;
                case Season.Winter:
                    dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        67,
                        229,
                        185
                    });
                    break;
            }
            Main.PlaySound(SoundID.Item10, projectile.position);
            int num3;
            for (int num795 = 4; num795 < 31; num795 = num3 + 1)
            {
                float num796 = projectile.oldVelocity.X * (30f / (float)num795);
                float num797 = projectile.oldVelocity.Y * (30f / (float)num795);
                int num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, dustType, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[num798].noGravity = true;
                Dust dust = Main.dust[num798];
                dust.velocity *= 0.5f;
                num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, dustType, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.4f);
                dust = Main.dust[num798];
                dust.velocity *= 0.05f;
                num3 = num795;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int buff = Main.dayTime ? BuffID.Daybreak : mod.BuffType("Nightwither");
            target.AddBuff(buff, 300);
        }
    }
}

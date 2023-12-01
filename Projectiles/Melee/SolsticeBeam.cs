using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class SolsticeBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item60, Projectile.position);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale -= 0.02f;
                Projectile.alpha += 30;
                if (Projectile.alpha >= 250)
                {
                    Projectile.alpha = 255;
                    Projectile.localAI[0] = 1f;
                }
            }
            else if (Projectile.localAI[0] == 1f)
            {
                Projectile.scale += 0.02f;
                Projectile.alpha -= 30;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[0] = 0f;
                }
            }
            int dustType = 0;

            switch (CalamityMod.CurrentSeason)
            {
                case Season.Spring:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        74,
                        157,
                        107
                    });
                    break;
                case Season.Summer:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        247,
                        228,
                        57
                    });
                    break;
                case Season.Fall:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        6,
                        259,
                        158
                    });
                    break;
                case Season.Winter:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        67,
                        229,
                        185
                    });
                    break;
            }
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f);
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
            return new Color(red, green, blue, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            int dustType = 0;

            switch (CalamityMod.CurrentSeason)
            {
                case Season.Spring:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        245,
                        157,
                        107
                    });
                    break;
                case Season.Summer:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        247,
                        228,
                        57
                    });
                    break;
                case Season.Fall:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        6,
                        259,
                        158
                    });
                    break;
                case Season.Winter:
                    dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        67,
                        229,
                        185
                    });
                    break;
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 27; i++)
            {
                float oldXVel = Projectile.oldVelocity.X * (30f / (float)i);
                float oldYVel = Projectile.oldVelocity.Y * (30f / (float)i);
                int solsticeDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - oldXVel, Projectile.oldPosition.Y - oldYVel), 8, 8, dustType, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Dust dust = Main.dust[solsticeDust];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                solsticeDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - oldXVel, Projectile.oldPosition.Y - oldYVel), 8, 8, dustType, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                dust.velocity *= 0.05f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int buff = Main.dayTime ? BuffID.Daybreak : ModContent.BuffType<Nightwither>();
            target.AddBuff(buff, 180);
        }
    }
}

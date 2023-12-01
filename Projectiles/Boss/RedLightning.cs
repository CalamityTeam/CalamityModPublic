using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace CalamityMod.Projectiles.Boss
{
    public class RedLightning : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/LightningProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 20;
            Projectile.timeLeft = 1260;
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
            if (Projectile.frameCounter == 0 || Projectile.oldPos[0] == Vector2.Zero)
            {
                for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
                {
                    Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                }
                Projectile.oldPos[0] = Projectile.position;
                if (Projectile.velocity == Vector2.Zero)
                {
                    float dustRotation = Projectile.rotation + MathHelper.PiOver2 + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                    float randDustRotateMod = (float)Main.rand.NextDouble() * 2f + 2f;
                    Vector2 dustVelocity = new Vector2((float)Math.Cos((double)dustRotation) * randDustRotateMod, (float)Math.Sin((double)dustRotation) * randDustRotateMod);
                    int redDust = Dust.NewDust(Projectile.oldPos[Projectile.oldPos.Length - 1], 0, 0, 60, dustVelocity.X, dustVelocity.Y, 0, default, 1f);
                    Main.dust[redDust].noGravity = true;
                    Main.dust[redDust].scale = 1.7f;
                }
            }

            int inc = Projectile.frameCounter;
            Projectile.frameCounter = inc + 1;
            Lighting.AddLight(Projectile.Center, 0.8f, 0.25f, 0.15f);
            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool lightningExpired = true;
                    for (int j = 1; j < Projectile.oldPos.Length; j = inc + 1)
                    {
                        if (Projectile.oldPos[j] != Projectile.oldPos[0])
                        {
                            lightningExpired = false;
                        }
                        inc = j;
                    }
                    if (lightningExpired)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (Main.rand.Next(Projectile.extraUpdates) == 0)
                {
                    for (int k = 0; k < 2; k = inc + 1)
                    {
                        float extraDustRotate = Projectile.rotation + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                        float extraRandRotate = (float)Main.rand.NextDouble() * 0.8f + 1f;
                        Vector2 dustVelocity = new Vector2((float)Math.Cos((double)extraDustRotate) * extraRandRotate, (float)Math.Sin((double)extraDustRotate) * extraRandRotate);
                        int extraRedDust = Dust.NewDust(Projectile.Center, 0, 0, 60, dustVelocity.X, dustVelocity.Y, 0, default, 1f);
                        Main.dust[extraRedDust].noGravity = true;
                        Main.dust[extraRedDust].scale = 1.2f;
                        inc = k;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        Vector2 moreDustRotation = Projectile.velocity.RotatedBy((double)MathHelper.PiOver2, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                        int moreExtraRedDust = Dust.NewDust(Projectile.Center + moreDustRotation - Vector2.One * 4f, 8, 8, 60, 0f, 0f, 100, default, 1.5f);
                        Dust dust = Main.dust[moreExtraRedDust];
                        dust.velocity *= 0.5f;
                        Main.dust[moreExtraRedDust].velocity.Y = -Math.Abs(Main.dust[moreExtraRedDust].velocity.Y);
                    }
                }
            }
            else if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
            {
                Projectile.frameCounter = 0;
                float projSpeed = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
                int frameIncrement = 0;
                Vector2 randomLightningMovement = -Vector2.UnitY;
                Vector2 lightningYRotation;
                do
                {
                    int randomIncrement = unifiedRandom.Next();
                    Projectile.ai[1] = (float)randomIncrement;
                    randomIncrement %= 100;
                    float f = (float)randomIncrement / 100f * MathHelper.TwoPi;
                    lightningYRotation = f.ToRotationVector2();
                    if (lightningYRotation.Y > 0f)
                    {
                        lightningYRotation.Y *= -1f;
                    }
                    bool stopLightning = false;
                    if (lightningYRotation.Y > -0.02f)
                    {
                        stopLightning = true;
                    }
                    if (lightningYRotation.X * (float)(Projectile.extraUpdates + 1) * 2f * projSpeed + Projectile.localAI[0] > 40f)
                    {
                        stopLightning = true;
                    }
                    if (lightningYRotation.X * (float)(Projectile.extraUpdates + 1) * 2f * projSpeed + Projectile.localAI[0] < -40f)
                    {
                        stopLightning = true;
                    }
                    if (!stopLightning)
                    {
                        goto IL_25086;
                    }
                    inc = frameIncrement;
                    frameIncrement = inc + 1;
                }
                while (inc < 100);

                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1] = 1f;

                goto IL_25092;

                IL_25086:
                randomLightningMovement = lightningYRotation;

                IL_25092:
                if (Projectile.velocity != Vector2.Zero)
                {
                    Projectile.localAI[0] += randomLightningMovement.X * (float)(Projectile.extraUpdates + 1) * 2f * projSpeed;
                    Projectile.velocity = randomLightningMovement.RotatedBy((double)(Projectile.ai[0] + MathHelper.PiOver2), default(Vector2)) * projSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.Electrified, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 end = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D tex3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/RedLightningTexture").Value;
            Projectile.GetAlpha(lightColor);
            Vector2 lightningScale = new Vector2(Projectile.scale) / 2f;
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    lightningScale = new Vector2(Projectile.scale) * 0.6f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(219, 104, 58, 0) * 0.5f;
                }
                else if (i == 1)
                {
                    lightningScale = new Vector2(Projectile.scale) * 0.4f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 126, 56, 0) * 0.5f;
                }
                else
                {
                    lightningScale = new Vector2(Projectile.scale) * 0.2f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 128, 128, 0) * 0.5f;
                }
                DelegateMethods.f_1 = 1f;
                for (int j = Projectile.oldPos.Length - 1; j > 0; j--)
                {
                    if (!(Projectile.oldPos[j] == Vector2.Zero))
                    {
                        Vector2 start = Projectile.oldPos[j] + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Vector2 end2 = Projectile.oldPos[j - 1] + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Utils.DrawLaser(Main.spriteBatch, tex3, start, end2, lightningScale, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                    }
                }
                if (Projectile.oldPos[0] != Vector2.Zero)
                {
                    DelegateMethods.f_1 = 1f;
                    Vector2 start2 = Projectile.oldPos[0] + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                    Utils.DrawLaser(Main.spriteBatch, tex3, start2, end, lightningScale, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                }
            }
            return false;
        }
    }
}

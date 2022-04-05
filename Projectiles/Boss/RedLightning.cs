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
    public class RedLightning : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LightningProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Lightning");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().canBreakPlayerDefense = true;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 20;
            Projectile.timeLeft = 1260;
            CooldownSlot = 1;
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
                for (int num31 = Projectile.oldPos.Length - 1; num31 > 0; num31--)
                {
                    Projectile.oldPos[num31] = Projectile.oldPos[num31 - 1];
                }
                Projectile.oldPos[0] = Projectile.position;
                if (Projectile.velocity == Vector2.Zero)
                {
                    float num32 = Projectile.rotation + MathHelper.PiOver2 + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                    float num33 = (float)Main.rand.NextDouble() * 2f + 2f;
                    Vector2 vector2 = new Vector2((float)Math.Cos((double)num32) * num33, (float)Math.Sin((double)num32) * num33);
                    int num34 = Dust.NewDust(Projectile.oldPos[Projectile.oldPos.Length - 1], 0, 0, 60, vector2.X, vector2.Y, 0, default, 1f);
                    Main.dust[num34].noGravity = true;
                    Main.dust[num34].scale = 1.7f;
                }
            }

            int num3 = Projectile.frameCounter;
            Projectile.frameCounter = num3 + 1;
            Lighting.AddLight(Projectile.Center, 0.8f, 0.25f, 0.15f);
            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool flag36 = true;
                    for (int num855 = 1; num855 < Projectile.oldPos.Length; num855 = num3 + 1)
                    {
                        if (Projectile.oldPos[num855] != Projectile.oldPos[0])
                        {
                            flag36 = false;
                        }
                        num3 = num855;
                    }
                    if (flag36)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (Main.rand.Next(Projectile.extraUpdates) == 0)
                {
                    for (int num856 = 0; num856 < 2; num856 = num3 + 1)
                    {
                        float num857 = Projectile.rotation + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                        float num858 = (float)Main.rand.NextDouble() * 0.8f + 1f;
                        Vector2 vector96 = new Vector2((float)Math.Cos((double)num857) * num858, (float)Math.Sin((double)num857) * num858);
                        int num859 = Dust.NewDust(Projectile.Center, 0, 0, 60, vector96.X, vector96.Y, 0, default, 1f);
                        Main.dust[num859].noGravity = true;
                        Main.dust[num859].scale = 1.2f;
                        num3 = num856;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        Vector2 value39 = Projectile.velocity.RotatedBy((double)MathHelper.PiOver2, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                        int num860 = Dust.NewDust(Projectile.Center + value39 - Vector2.One * 4f, 8, 8, 60, 0f, 0f, 100, default, 1.5f);
                        Dust dust = Main.dust[num860];
                        dust.velocity *= 0.5f;
                        Main.dust[num860].velocity.Y = -Math.Abs(Main.dust[num860].velocity.Y);
                    }
                }
            }
            else if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
            {
                Projectile.frameCounter = 0;
                float num861 = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
                int num862 = 0;
                Vector2 vector97 = -Vector2.UnitY;
                Vector2 vector98;
                do
                {
                    int num863 = unifiedRandom.Next();
                    Projectile.ai[1] = (float)num863;
                    num863 %= 100;
                    float f = (float)num863 / 100f * MathHelper.TwoPi;
                    vector98 = f.ToRotationVector2();
                    if (vector98.Y > 0f)
                    {
                        vector98.Y *= -1f;
                    }
                    bool flag37 = false;
                    if (vector98.Y > -0.02f)
                    {
                        flag37 = true;
                    }
                    if (vector98.X * (float)(Projectile.extraUpdates + 1) * 2f * num861 + Projectile.localAI[0] > 40f)
                    {
                        flag37 = true;
                    }
                    if (vector98.X * (float)(Projectile.extraUpdates + 1) * 2f * num861 + Projectile.localAI[0] < -40f)
                    {
                        flag37 = true;
                    }
                    if (!flag37)
                    {
                        goto IL_25086;
                    }
                    num3 = num862;
                    num862 = num3 + 1;
                }
                while (num3 < 100);

                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1] = 1f;

                goto IL_25092;

                IL_25086:
                vector97 = vector98;

                IL_25092:
                if (Projectile.velocity != Vector2.Zero)
                {
                    Projectile.localAI[0] += vector97.X * (float)(Projectile.extraUpdates + 1) * 2f * num861;
                    Projectile.velocity = vector97.RotatedBy((double)(Projectile.ai[0] + MathHelper.PiOver2), default(Vector2)) * num861;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 end = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D tex3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/RedLightningTexture");
            Projectile.GetAlpha(lightColor);
            Vector2 scale16 = new Vector2(Projectile.scale) / 2f;
            for (int num289 = 0; num289 < 3; num289++)
            {
                if (num289 == 0)
                {
                    scale16 = new Vector2(Projectile.scale) * 0.6f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(219, 104, 58, 0) * 0.5f;
                }
                else if (num289 == 1)
                {
                    scale16 = new Vector2(Projectile.scale) * 0.4f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 126, 56, 0) * 0.5f;
                }
                else
                {
                    scale16 = new Vector2(Projectile.scale) * 0.2f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 128, 128, 0) * 0.5f;
                }
                DelegateMethods.f_1 = 1f;
                for (int num290 = Projectile.oldPos.Length - 1; num290 > 0; num290--)
                {
                    if (!(Projectile.oldPos[num290] == Vector2.Zero))
                    {
                        Vector2 start = Projectile.oldPos[num290] + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Vector2 end2 = Projectile.oldPos[num290 - 1] + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Utils.DrawLaser(spriteBatch, tex3, start, end2, scale16, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                    }
                }
                if (Projectile.oldPos[0] != Vector2.Zero)
                {
                    DelegateMethods.f_1 = 1f;
                    Vector2 start2 = Projectile.oldPos[0] + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                    Utils.DrawLaser(Main.spriteBatch, tex3, start2, end, scale16, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                }
            }
            return false;
        }
    }
}

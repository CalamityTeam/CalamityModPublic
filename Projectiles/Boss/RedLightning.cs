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
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 14;
            projectile.height = 14;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 20;
            projectile.timeLeft = 1260;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.frameCounter == 0 || projectile.oldPos[0] == Vector2.Zero)
            {
                for (int num31 = projectile.oldPos.Length - 1; num31 > 0; num31--)
                {
                    projectile.oldPos[num31] = projectile.oldPos[num31 - 1];
                }
                projectile.oldPos[0] = projectile.position;
                if (projectile.velocity == Vector2.Zero)
                {
                    float num32 = projectile.rotation + MathHelper.PiOver2 + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                    float num33 = (float)Main.rand.NextDouble() * 2f + 2f;
                    Vector2 vector2 = new Vector2((float)Math.Cos((double)num32) * num33, (float)Math.Sin((double)num32) * num33);
                    int num34 = Dust.NewDust(projectile.oldPos[projectile.oldPos.Length - 1], 0, 0, 60, vector2.X, vector2.Y, 0, default, 1f);
                    Main.dust[num34].noGravity = true;
                    Main.dust[num34].scale = 1.7f;
                }
            }

            int num3 = projectile.frameCounter;
            projectile.frameCounter = num3 + 1;
            Lighting.AddLight(projectile.Center, 0.8f, 0.25f, 0.15f);
            if (projectile.velocity == Vector2.Zero)
            {
                if (projectile.frameCounter >= projectile.extraUpdates * 2)
                {
                    projectile.frameCounter = 0;
                    bool flag36 = true;
                    for (int num855 = 1; num855 < projectile.oldPos.Length; num855 = num3 + 1)
                    {
                        if (projectile.oldPos[num855] != projectile.oldPos[0])
                        {
                            flag36 = false;
                        }
                        num3 = num855;
                    }
                    if (flag36)
                    {
                        projectile.Kill();
                        return;
                    }
                }
                if (Main.rand.Next(projectile.extraUpdates) == 0)
                {
                    for (int num856 = 0; num856 < 2; num856 = num3 + 1)
                    {
                        float num857 = projectile.rotation + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                        float num858 = (float)Main.rand.NextDouble() * 0.8f + 1f;
                        Vector2 vector96 = new Vector2((float)Math.Cos((double)num857) * num858, (float)Math.Sin((double)num857) * num858);
                        int num859 = Dust.NewDust(projectile.Center, 0, 0, 60, vector96.X, vector96.Y, 0, default, 1f);
                        Main.dust[num859].noGravity = true;
                        Main.dust[num859].scale = 1.2f;
                        num3 = num856;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        Vector2 value39 = projectile.velocity.RotatedBy((double)MathHelper.PiOver2, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                        int num860 = Dust.NewDust(projectile.Center + value39 - Vector2.One * 4f, 8, 8, 60, 0f, 0f, 100, default, 1.5f);
                        Dust dust = Main.dust[num860];
                        dust.velocity *= 0.5f;
                        Main.dust[num860].velocity.Y = -Math.Abs(Main.dust[num860].velocity.Y);
                    }
                }
            }
            else if (projectile.frameCounter >= projectile.extraUpdates * 2)
            {
                projectile.frameCounter = 0;
                float num861 = projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)projectile.ai[1]);
                int num862 = 0;
                Vector2 vector97 = -Vector2.UnitY;
                Vector2 vector98;
                do
                {
                    int num863 = unifiedRandom.Next();
                    projectile.ai[1] = (float)num863;
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
                    if (vector98.X * (float)(projectile.extraUpdates + 1) * 2f * num861 + projectile.localAI[0] > 40f)
                    {
                        flag37 = true;
                    }
                    if (vector98.X * (float)(projectile.extraUpdates + 1) * 2f * num861 + projectile.localAI[0] < -40f)
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

                projectile.velocity = Vector2.Zero;
                projectile.localAI[1] = 1f;

                goto IL_25092;

                IL_25086:
                vector97 = vector98;

                IL_25092:
                if (projectile.velocity != Vector2.Zero)
                {
                    projectile.localAI[0] += vector97.X * (float)(projectile.extraUpdates + 1) * 2f * num861;
                    projectile.velocity = vector97.RotatedBy((double)(projectile.ai[0] + MathHelper.PiOver2), default(Vector2)) * num861;
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 end = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D tex3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/RedLightningTexture");
            projectile.GetAlpha(lightColor);
            Vector2 scale16 = new Vector2(projectile.scale) / 2f;
            for (int num289 = 0; num289 < 3; num289++)
            {
                if (num289 == 0)
                {
                    scale16 = new Vector2(projectile.scale) * 0.6f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(219, 104, 58, 0) * 0.5f;
                }
                else if (num289 == 1)
                {
                    scale16 = new Vector2(projectile.scale) * 0.4f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 126, 56, 0) * 0.5f;
                }
                else
                {
                    scale16 = new Vector2(projectile.scale) * 0.2f;
                    DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 128, 128, 0) * 0.5f;
                }
                DelegateMethods.f_1 = 1f;
                for (int num290 = projectile.oldPos.Length - 1; num290 > 0; num290--)
                {
                    if (!(projectile.oldPos[num290] == Vector2.Zero))
                    {
                        Vector2 start = projectile.oldPos[num290] + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
                        Vector2 end2 = projectile.oldPos[num290 - 1] + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
                        Utils.DrawLaser(spriteBatch, tex3, start, end2, scale16, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                    }
                }
                if (projectile.oldPos[0] != Vector2.Zero)
                {
                    DelegateMethods.f_1 = 1f;
                    Vector2 start2 = projectile.oldPos[0] + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
                    Utils.DrawLaser(Main.spriteBatch, tex3, start2, end, scale16, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                }
            }
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SilvaCrystalExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Color newColor2 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            int num978 = (int)Projectile.ai[1];
            if (num978 < 0 || num978 >= 1000 || (!Main.projectile[num978].active && Main.projectile[num978].type != ModContent.ProjectileType<SilvaCrystal>()))
            {
                Projectile.ai[1] = -1f;
            }
            else
            {
                DelegateMethods.v3_1 = newColor2.ToVector3() * 0.5f;
                Utils.PlotTileLine(Projectile.Center, Main.projectile[num978].Center, 8f, DelegateMethods.CastLight);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Main.rand.NextFloat() * 0.8f + 0.8f;
                Projectile.direction = (Main.rand.Next(2) > 0) ? 1 : -1;
            }
            Projectile.rotation = Projectile.localAI[1] / 20f * 6.28318548f * (float)Projectile.direction;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha == 0)
            {
                Lighting.AddLight(Projectile.Center, newColor2.ToVector3() * 0.5f);
            }
            int num3;
            for (int num979 = 0; num979 < 2; num979 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value55 = Vector2.UnitY.RotatedBy((double)((float)num979 * 3.14159274f), default).RotatedBy((double)Projectile.rotation, default);
                    Dust dust24 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust24.noGravity = true;
                    dust24.noLight = true;
                    dust24.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust24.position = Projectile.Center;
                    dust24.velocity = value55 * 2.5f;
                }
                num3 = num979;
            }
            for (int num980 = 0; num980 < 2; num980 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value56 = Vector2.UnitY.RotatedBy((double)((float)num980 * 3.14159274f), default);
                    Dust dust25 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust25.noGravity = true;
                    dust25.noLight = true;
                    dust25.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust25.position = Projectile.Center;
                    dust25.velocity = value56 * 2.5f;
                }
                num3 = num980;
            }
            if (Main.rand.NextBool(10))
            {
                float scaleFactor13 = 1f + Main.rand.NextFloat() * 2f;
                float fadeIn = 1f + Main.rand.NextFloat();
                float num981 = 1f + Main.rand.NextFloat();
                Vector2 vector136 = Utils.RandomVector2(Main.rand, -1f, 1f);
                if (vector136 != Vector2.Zero)
                {
                    vector136.Normalize();
                }
                vector136 *= 20f + Main.rand.NextFloat() * 100f;
                Vector2 vector137 = Projectile.Center + vector136;
                Point point3 = vector137.ToTileCoordinates();
                bool flag52 = true;
                if (!WorldGen.InWorld(point3.X, point3.Y, 0))
                {
                    flag52 = false;
                }
                if (flag52 && WorldGen.SolidTile(point3.X, point3.Y))
                {
                    flag52 = false;
                }
                if (flag52)
                {
                    Dust dust26 = Main.dust[Dust.NewDust(vector137, 0, 0, 267, 0f, 0f, 127, newColor2, 1f)];
                    dust26.noGravity = true;
                    dust26.position = vector137;
                    dust26.velocity = -Vector2.UnitY * scaleFactor13 * (Main.rand.NextFloat() * 0.9f + 1.6f);
                    dust26.fadeIn = fadeIn;
                    dust26.scale = num981;
                    dust26.noLight = true;
                    Dust dust27 = Dust.CloneDust(dust26);
                    Dust dust = dust27;
                    dust.scale *= 0.65f;
                    dust = dust27;
                    dust.fadeIn *= 0.65f;
                    dust27.color = new Color(255, 255, 255, 255);
                }
            }
            Projectile.scale = Projectile.Opacity / 2f * Projectile.localAI[0];
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] >= 30f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            Vector2 vector59 = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangle17 = texture2D34.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color alpha5 = Projectile.GetAlpha(color25);
            Vector2 origin11 = rectangle17.Size() / 2f;
            Color color60 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f).MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.spriteBatch.Draw(texture2D34, vector59, new Microsoft.Xna.Framework.Rectangle?(rectangle17), color60, Projectile.rotation, origin11, Projectile.scale * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture2D34, vector59, new Microsoft.Xna.Framework.Rectangle?(rectangle17), color60, 0f, origin11, Projectile.scale * 2f, SpriteEffects.None, 0);
            if (Projectile.ai[1] != -1f && Projectile.Opacity > 0.3f)
            {
                Vector2 vector61 = Main.projectile[(int)Projectile.ai[1]].Center - Projectile.Center;
                Vector2 vector62 = new Vector2(1f, vector61.Length() / (float)texture2D34.Height);
                float rotation27 = vector61.ToRotation() + 1.57079637f;
                float num279 = MathHelper.Distance(15f, Projectile.localAI[1]) / 20f;
                num279 = MathHelper.Clamp(num279, 0f, 1f);
                if (num279 > 0f)
                {
                    Main.spriteBatch.Draw(texture2D34, vector59 + vector61 / 2f, new Microsoft.Xna.Framework.Rectangle?(rectangle17), color60 * num279, rotation27, origin11, vector62, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texture2D34, vector59 + vector61 / 2f, new Microsoft.Xna.Framework.Rectangle?(rectangle17), alpha5 * num279, rotation27, origin11, vector62 / 2f, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float num69 = (float)Main.rand.Next(7, 13);
            Vector2 value5 = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            newColor.A = 255;
            float num72;
            for (float num70 = 0f; num70 < num69; num70 = num72 + 1f)
            {
                int num71 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num71].position = Projectile.Center;
                Main.dust[num71].velocity = spinningpoint.RotatedBy((double)(6.28318548f * num70 / num69), default) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[num71].noGravity = true;
                Main.dust[num71].scale = 2f;
                Main.dust[num71].fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust11 = Dust.CloneDust(num71);
                Dust dust = dust11;
                dust.scale /= 2f;
                dust = dust11;
                dust.fadeIn /= 2f;
                dust11.color = new Color(255, 255, 255, 255);
                num72 = num70;
            }
            for (float num73 = 0f; num73 < num69; num73 = num72 + 1f)
            {
                int num74 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num74].position = Projectile.Center;
                Main.dust[num74].velocity = spinningpoint.RotatedBy((double)(6.28318548f * num73 / num69), default) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Main.dust[num74];
                dust.velocity *= Main.rand.NextFloat() * 0.8f;
                Main.dust[num74].noGravity = true;
                Main.dust[num74].scale = Main.rand.NextFloat() * 1f;
                Main.dust[num74].fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust12 = Dust.CloneDust(num74);
                dust = dust12;
                dust.scale /= 2f;
                dust = dust12;
                dust.fadeIn /= 2f;
                dust12.color = new Color(255, 255, 255, 255);
                num72 = num73;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.friendly = true;
                int width = Projectile.width;
                int height2 = Projectile.height;
                int num75 = Projectile.penetrate;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 60;
                Projectile.Center = Projectile.position;
                Projectile.penetrate = -1;
                Projectile.maxPenetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.Damage();
                Projectile.penetrate = num75;
                Projectile.position = Projectile.Center;
                Projectile.width = width;
                Projectile.height = height2;
                Projectile.Center = Projectile.position;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
    }
}

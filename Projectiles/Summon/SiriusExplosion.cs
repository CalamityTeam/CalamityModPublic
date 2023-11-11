using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.Projectiles.Summon
{
    public class SiriusExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Color newColor2 = Main.hslToRgb(0.5f, 1f, Projectile.ai[0]);
            int activeTimer = (int)Projectile.ai[1];
            if (activeTimer < 0 || activeTimer >= 1000 || (!Main.projectile[activeTimer].active && Main.projectile[activeTimer].type != ModContent.ProjectileType<SilvaCrystal>()))
            {
                Projectile.ai[1] = -1f;
            }
            else
            {
                DelegateMethods.v3_1 = newColor2.ToVector3() * 0.5f;
                Utils.PlotTileLine(Projectile.Center, Main.projectile[activeTimer].Center, 8f, DelegateMethods.CastLight);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Main.rand.NextFloat() * 0.8f + 0.8f;
                Projectile.direction = (Main.rand.Next(2) > 0) ? 1 : -1;
            }
            Projectile.rotation = Projectile.localAI[1] / 40f * 6.28318548f * (float)Projectile.direction;
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
            int dustCount;
            for (int i = 0; i < 2; i = dustCount + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 siriusDustVel = Vector2.UnitY.RotatedBy((double)((float)i * 3.14159274f), default).RotatedBy((double)Projectile.rotation, default);
                    Dust siriusDust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    siriusDust.noGravity = true;
                    siriusDust.noLight = true;
                    siriusDust.scale = Projectile.Opacity * Projectile.localAI[0];
                    siriusDust.position = Projectile.Center;
                    siriusDust.velocity = siriusDustVel * 2.5f;
                }
                dustCount = i;
            }
            for (int j = 0; j < 2; j = dustCount + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 siriusDustVel2 = Vector2.UnitY.RotatedBy((double)((float)j * 3.14159274f), default);
                    Dust siriusDust2 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    siriusDust2.noGravity = true;
                    siriusDust2.noLight = true;
                    siriusDust2.scale = Projectile.Opacity * Projectile.localAI[0];
                    siriusDust2.position = Projectile.Center;
                    siriusDust2.velocity = siriusDustVel2 * 2.5f;
                }
                dustCount = j;
            }
            if (Main.rand.NextBool(10))
            {
                float dustVelMod = 1f + Main.rand.NextFloat() * 2f;
                float fadeIn = 1f + Main.rand.NextFloat();
                float dustScale = 1f + Main.rand.NextFloat();
                Vector2 randVector = Utils.RandomVector2(Main.rand, -1f, 1f);
                if (randVector != Vector2.Zero)
                {
                    randVector.Normalize();
                }
                randVector *= 20f + Main.rand.NextFloat() * 100f;
                Vector2 dustPos = Projectile.Center + randVector;
                Point dustCoords = dustPos.ToTileCoordinates();
                bool shouldSpawnDust = true;
                if (!WorldGen.InWorld(dustCoords.X, dustCoords.Y, 0))
                {
                    shouldSpawnDust = false;
                }
                if (shouldSpawnDust && WorldGen.SolidTile(dustCoords.X, dustCoords.Y))
                {
                    shouldSpawnDust = false;
                }
                if (shouldSpawnDust)
                {
                    Dust starDust = Main.dust[Dust.NewDust(dustPos, 0, 0, 267, 0f, 0f, 127, newColor2, 1f)];
                    starDust.noGravity = true;
                    starDust.position = dustPos;
                    starDust.velocity = -Vector2.UnitY * dustVelMod * (Main.rand.NextFloat() * 0.9f + 1.6f);
                    starDust.fadeIn = fadeIn;
                    starDust.scale = dustScale;
                    starDust.noLight = true;
                    Dust starDust2 = Dust.CloneDust(starDust);
                    Dust dust = starDust2;
                    dust.scale *= 0.65f;
                    dust = starDust2;
                    dust.fadeIn *= 0.65f;
                    starDust2.color = new Color(255, 255, 255, 255);
                }
            }
            Projectile.scale = Projectile.Opacity / 2f * Projectile.localAI[0];
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] >= 60f)
            {
                Projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color colorArea = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            Vector2 projPos = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangular = texture2D34.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color colorAlpha = Projectile.GetAlpha(colorArea);
            Vector2 halfRectangle = rectangular.Size() / 2f;
            Color projColor = Main.hslToRgb(0.5f, 1f, Projectile.ai[0]).MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.EntitySpriteDraw(texture2D34, projPos, new Microsoft.Xna.Framework.Rectangle?(rectangular), projColor, Projectile.rotation, halfRectangle, Projectile.scale * 2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture2D34, projPos, new Microsoft.Xna.Framework.Rectangle?(rectangular), projColor, 0f, halfRectangle, Projectile.scale * 2f, SpriteEffects.None, 0);
            if (Projectile.ai[1] != -1f && Projectile.Opacity > 0.3f)
            {
                Vector2 projDirection = Main.projectile[(int)Projectile.ai[1]].Center - Projectile.Center;
                Vector2 projDistance = new Vector2(1f, projDirection.Length() / (float)texture2D34.Height);
                float drawRotation = projDirection.ToRotation() + 1.57079637f;
                float colorClamp = MathHelper.Distance(30f, Projectile.localAI[1]) / 20f;
                colorClamp = MathHelper.Clamp(colorClamp, 0f, 1f);
                if (colorClamp > 0f)
                {
                    Main.EntitySpriteDraw(texture2D34, projPos + projDirection / 2f, new Microsoft.Xna.Framework.Rectangle?(rectangular), projColor * colorClamp, drawRotation, halfRectangle, projDistance, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture2D34, projPos + projDirection / 2f, new Microsoft.Xna.Framework.Rectangle?(rectangular), colorAlpha * colorClamp, drawRotation, halfRectangle, projDistance / 2f, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float rando = (float)Main.rand.Next(7, 13);
            Vector2 dustVel = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(0.5f, 1f, Projectile.ai[0]);
            newColor.A = 255;
            float dustCount;
            for (float i = 0f; i < rando; i = dustCount + 1f)
            {
                int dusty = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[dusty].position = Projectile.Center;
                Main.dust[dusty].velocity = spinningpoint.RotatedBy((double)(6.28318548f * i / rando), default) * dustVel * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].scale = 2f;
                Main.dust[dusty].fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust11 = Dust.CloneDust(dusty);
                Dust dust = dust11;
                dust.scale /= 2f;
                dust = dust11;
                dust.fadeIn /= 2f;
                dust11.color = new Color(255, 255, 255, 255);
                dustCount = i;
            }
            for (float j = 0f; j < rando; j = dustCount + 1f)
            {
                int dusty2 = Dust.NewDust(Projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[dusty2].position = Projectile.Center;
                Main.dust[dusty2].velocity = spinningpoint.RotatedBy((double)(6.28318548f * j / rando), default) * dustVel * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Main.dust[dusty2];
                dust.velocity *= Main.rand.NextFloat() * 0.8f;
                Main.dust[dusty2].noGravity = true;
                Main.dust[dusty2].scale = Main.rand.NextFloat() * 1f;
                Main.dust[dusty2].fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust12 = Dust.CloneDust(dusty2);
                dust = dust12;
                dust.scale /= 2f;
                dust = dust12;
                dust.fadeIn /= 2f;
                dust12.color = new Color(255, 255, 255, 255);
                dustCount = j;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.friendly = true;
                int width = Projectile.width;
                int height2 = Projectile.height;
                int penetrateClone = Projectile.penetrate;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 60;
                Projectile.Center = Projectile.position;
                Projectile.penetrate = -1;
                Projectile.maxPenetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.Damage();
                Projectile.penetrate = penetrateClone;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }
}

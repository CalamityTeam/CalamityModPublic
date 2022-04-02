using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class ProvidenceCrystalShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Crystal Shard");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 34;
            projectile.height = 34;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
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
            if (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive)
                projectile.extraUpdates = 1;

            if (projectile.timeLeft < 300)
            {
                projectile.tileCollide = true;
            }
            Color newColor2 = Main.hslToRgb(projectile.ai[0], 1f, 0.5f);
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 8;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.alpha == 0)
            {
                Lighting.AddLight(projectile.Center, newColor2.ToVector3() * 0.5f);
            }
            projectile.velocity.X *= 0.995f;
            if (projectile.velocity.Y < 0f)
            {
                projectile.velocity.Y *= 0.98f;
            }
            else
            {
                projectile.velocity.Y *= 1.06f;
                float fallSpeed = (CalamityWorld.revenge || BossRushEvent.BossRushActive || !Main.dayTime) ? 3.5f : 3f;
                if (projectile.velocity.Y > fallSpeed)
                {
                    projectile.velocity.Y = fallSpeed;
                }
            }
            if (projectile.velocity.Y > -0.5f && projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1f;
                projectile.velocity.Y = 0.5f;
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 1.57f;
            int num3;
            for (int num979 = 0; num979 < 2; num979 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value55 = Vector2.UnitY.RotatedBy(num979 * 3.14159274f).RotatedBy(projectile.rotation);
                    Dust dust24 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust24.noGravity = true;
                    dust24.noLight = true;
                    dust24.scale = projectile.Opacity * projectile.localAI[0];
                    dust24.position = projectile.Center;
                    dust24.velocity = value55 * 2.5f;
                }
                num3 = num979;
            }
            for (int num980 = 0; num980 < 2; num980 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value56 = Vector2.UnitY.RotatedBy(num980 * 3.14159274f);
                    Dust dust25 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 225, newColor2, 1.5f)];
                    dust25.noGravity = true;
                    dust25.noLight = true;
                    dust25.scale = projectile.Opacity * projectile.localAI[0];
                    dust25.position = projectile.Center;
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
                Vector2 vector137 = projectile.Center + vector136;
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
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float num69 = Main.rand.Next(7, 13);
            Vector2 value5 = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(projectile.ai[0], 1f, 0.5f);
            newColor.A = 255;
            float num72;
            for (float num70 = 0f; num70 < num69; num70 = num72 + 1f)
            {
                int num71 = Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num71].position = projectile.Center;
                Main.dust[num71].velocity = spinningpoint.RotatedBy(6.28318548f * num70 / num69) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
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
                int num74 = Dust.NewDust(projectile.Center, 0, 0, 267, 0f, 0f, 0, newColor, 1f);
                Main.dust[num74].position = projectile.Center;
                Main.dust[num74].velocity = spinningpoint.RotatedBy(6.28318548f * num73 / num69) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Main.dust[num74];
                dust.velocity *= Main.rand.NextFloat() * 0.8f;
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat() * 1f;
                dust.fadeIn = Main.rand.NextFloat() * 2f;
                Dust dust12 = Dust.CloneDust(num74);
                dust = dust12;
                dust.scale /= 2f;
                dust = dust12;
                dust.fadeIn /= 2f;
                dust12.color = new Color(255, 255, 255, 255);
                num72 = num73;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}

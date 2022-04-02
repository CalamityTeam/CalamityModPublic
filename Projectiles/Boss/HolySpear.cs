using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolySpear : ModProjectile
    {
        Vector2 velocity = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.WriteVector2(velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            velocity = reader.ReadVector2();
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;

                if (projectile.ai[0] == 1f)
                    velocity = projectile.velocity;
            }

            float timeGateValue = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 420f : 540f;
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;

                float slowGateValue = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 60f : 90f;
                float fastGateValue = 30f;
                float minVelocity = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 4f : 3f;
                float maxVelocity = minVelocity * 4f;
                float extremeVelocity = maxVelocity * 2f;
                float deceleration = 0.95f;
                float acceleration = 1.2f;

                if (projectile.localAI[1] >= timeGateValue)
                {
                    if (projectile.velocity.Length() < extremeVelocity)
                        projectile.velocity *= acceleration;
                }
                else
                {
                    if (projectile.ai[1] <= slowGateValue)
                    {
                        if (projectile.velocity.Length() > minVelocity)
                            projectile.velocity *= deceleration;
                    }
                    else if (projectile.ai[1] < slowGateValue + fastGateValue)
                    {
                        if (projectile.velocity.Length() < maxVelocity)
                            projectile.velocity *= acceleration;
                    }
                    else
                        projectile.ai[1] = 0f;
                }
            }
            else
            {
                float frequency = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 0.2f : 0.1f;
                float amplitude = (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 4f : 2f;

                projectile.ai[1] += frequency;

                float wavyVelocity = (float)Math.Sin(projectile.ai[1]);

                projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity).RotatedBy(MathHelper.ToRadians(velocity.ToRotation())) * amplitude;
            }

            if (projectile.localAI[1] < timeGateValue)
            {
                projectile.localAI[1] += 1f;

                if (projectile.timeLeft < 160)
                    projectile.timeLeft = 160;
            }

            projectile.Opacity = MathHelper.Lerp(240f, 220f, projectile.timeLeft);

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D value = Main.projectileTexture[projectile.type];
            int green = projectile.ai[0] != 0f ? 255 : 125;
            int blue = projectile.ai[0] != 0f ? 0 : 125;
            Color baseColor = new Color(255, green, blue, 255);

            if (!Main.dayTime || CalamityWorld.malice)
            {
                int red = projectile.ai[0] != 0f ? 100 : 175;
                green = projectile.ai[0] != 0f ? 255 : 175;
                baseColor = new Color(red, green, 255, 255);
            }

            Color color33 = baseColor * 0.5f;
            color33.A = 0;
            Vector2 vector28 = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            Color color34 = color33;
            Vector2 origin5 = value.Size() / 2f;
            Color color35 = color33 * 0.5f;
            float num162 = Utils.InverseLerp(15f, 30f, projectile.timeLeft, clamped: true) * Utils.InverseLerp(240f, 200f, projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 vector29 = new Vector2(1f, 1.5f) * num162;
            Vector2 vector30 = new Vector2(0.5f, 1f) * num162;
            color34 *= num162;
            color35 *= num162;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = projectile.oldPos[i] + vector28;
                    Color color = projectile.GetAlpha(color34) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);
                    Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector29, spriteEffects, 0f);
                    Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector30, spriteEffects, 0f);

                    color = projectile.GetAlpha(color35) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);
                    Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector29 * 0.6f, spriteEffects, 0f);
                    Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector30 * 0.6f, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(value, vector28, null, color34, projectile.rotation, origin5, vector29, spriteEffects, 0);
            spriteBatch.Draw(value, vector28, null, color34, projectile.rotation, origin5, vector30, spriteEffects, 0);
            spriteBatch.Draw(value, vector28, null, color35, projectile.rotation, origin5, vector29 * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(value, vector28, null, color35, projectile.rotation, origin5, vector30 * 0.6f, spriteEffects, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = (Main.dayTime && !CalamityWorld.malice) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}

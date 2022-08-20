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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.WriteVector2(velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            velocity = reader.ReadVector2();
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                if (Projectile.ai[0] == 1f)
                    velocity = Projectile.velocity;
            }

            float timeGateValue = (!Main.dayTime || BossRushEvent.BossRushActive) ? 420f : 540f;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;

                float slowGateValue = (!Main.dayTime || BossRushEvent.BossRushActive) ? 60f : 90f;
                float fastGateValue = 30f;
                float minVelocity = (!Main.dayTime || BossRushEvent.BossRushActive) ? 4f : 3f;
                float maxVelocity = minVelocity * 4f;
                float extremeVelocity = maxVelocity * 2f;
                float deceleration = 0.95f;
                float acceleration = 1.2f;

                if (Projectile.localAI[1] >= timeGateValue)
                {
                    if (Projectile.velocity.Length() < extremeVelocity)
                        Projectile.velocity *= acceleration;
                }
                else
                {
                    if (Projectile.ai[1] <= slowGateValue)
                    {
                        if (Projectile.velocity.Length() > minVelocity)
                            Projectile.velocity *= deceleration;
                    }
                    else if (Projectile.ai[1] < slowGateValue + fastGateValue)
                    {
                        if (Projectile.velocity.Length() < maxVelocity)
                            Projectile.velocity *= acceleration;
                    }
                    else
                        Projectile.ai[1] = 0f;
                }
            }
            else
            {
                float frequency = (!Main.dayTime || BossRushEvent.BossRushActive) ? 0.2f : 0.1f;
                float amplitude = (!Main.dayTime || BossRushEvent.BossRushActive) ? 4f : 2f;

                Projectile.ai[1] += frequency;

                float wavyVelocity = (float)Math.Sin(Projectile.ai[1]);

                Projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity).RotatedBy(MathHelper.ToRadians(velocity.ToRotation())) * amplitude;
            }

            if (Projectile.localAI[1] < timeGateValue)
            {
                Projectile.localAI[1] += 1f;

                if (Projectile.timeLeft < 160)
                    Projectile.timeLeft = 160;
            }

            Projectile.Opacity = MathHelper.Lerp(240f, 220f, Projectile.timeLeft);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>(Texture).Value;
            int green = Projectile.ai[0] != 0f ? 255 : 125;
            int blue = Projectile.ai[0] != 0f ? 0 : 125;
            Color baseColor = new Color(255, green, blue, 255);

            if (!Main.dayTime || BossRushEvent.BossRushActive)
            {
                int red = Projectile.ai[0] != 0f ? 100 : 175;
                green = Projectile.ai[0] != 0f ? 255 : 175;
                baseColor = new Color(red, green, 255, 255);
            }

            Color color33 = baseColor * 0.5f;
            color33.A = 0;
            Vector2 vector28 = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color34 = color33;
            Vector2 origin5 = value.Size() / 2f;
            Color color35 = color33 * 0.5f;
            float num162 = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 vector29 = new Vector2(1f, 1.5f) * num162;
            Vector2 vector30 = new Vector2(0.5f, 1f) * num162;
            color34 *= num162;
            color35 *= num162;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + vector28;
                    Color color = Projectile.GetAlpha(color34) * ((Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(value, drawPos, null, color, Projectile.rotation, origin5, vector29, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(value, drawPos, null, color, Projectile.rotation, origin5, vector30, SpriteEffects.None, 0);

                    color = Projectile.GetAlpha(color35) * ((Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(value, drawPos, null, color, Projectile.rotation, origin5, vector29 * 0.6f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(value, drawPos, null, color, Projectile.rotation, origin5, vector30 * 0.6f, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(value, vector28, null, color34, Projectile.rotation, origin5, vector29, spriteEffects, 0);
            Main.EntitySpriteDraw(value, vector28, null, color34, Projectile.rotation, origin5, vector30, spriteEffects, 0);
            Main.EntitySpriteDraw(value, vector28, null, color35, Projectile.rotation, origin5, vector29 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(value, vector28, null, color35, Projectile.rotation, origin5, vector30 * 0.6f, spriteEffects, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = (Main.dayTime && !BossRushEvent.BossRushActive) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 180);
        }
    }
}

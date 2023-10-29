using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
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
    public class HolySpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        Vector2 velocity = Vector2.Zero;

        public override void SetStaticDefaults()
        {
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
            Lighting.AddLight(Projectile.Center, 0.45f * Projectile.Opacity, 0.35f * Projectile.Opacity, 0f);

            // Day mode by default but syncs with the boss
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                if (Projectile.ai[0] == 1f)
                    velocity = Projectile.velocity;
            }

            bool commanderSpear = Projectile.ai[0] == -1f;
            bool enragedCommanderSpear = Projectile.ai[0] == -2f;
            float timeGateValue = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 420f : ((commanderSpear || enragedCommanderSpear) ? 360f : 540f);
            if (Projectile.ai[0] <= 0f)
            {
                Projectile.ai[1] += 1f;

                float slowGateValue = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 60f : ((commanderSpear || enragedCommanderSpear) ? 30f : 90f);
                float fastGateValue = 30f;
                float minVelocity = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 4f : (enragedCommanderSpear ? 6f : commanderSpear ? 4.5f : 3f);
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
                float frequency = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 0.2f : 0.1f;
                float amplitude = (Projectile.maxPenetrate != (int)Providence.BossMode.Day) ? 4f : 2f;

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
            Texture2D drawTexture = ModContent.Request<Texture2D>(Texture).Value;
            bool aimedSpear = Projectile.ai[0] > 0f;

            int red = 255;
            int green = 255;
            int blue = 0;
            switch (Projectile.maxPenetrate)
            {
                case (int)Providence.BossMode.Red:
                    red = 255;
                    green = aimedSpear ? 0 : 125;
                    blue = aimedSpear ? 0 : 255;
                    break;
                case (int)Providence.BossMode.Orange:
                    red = 255;
                    green = 125;
                    blue = aimedSpear ? 0 : 175;
                    break;
                case (int)Providence.BossMode.Yellow: //Same as day
                case (int)Providence.BossMode.Day:
                    red = 255;
                    green = aimedSpear ? 255 : 125;
                    blue = aimedSpear ? 0 : 125;
                    break;
                case (int)Providence.BossMode.Green:
                    red = 0;
                    green = 255;
                    blue = aimedSpear ? 0 : 175;
                    break;
                case (int)Providence.BossMode.Blue: //Same as night
                case (int)Providence.BossMode.Night:
                    red = aimedSpear ? 100 : 175;
                    green = aimedSpear ? 255 : 175;
                    blue = 255;
                    break;
                case (int)Providence.BossMode.Violet:
                    red = aimedSpear ? 125 : 255;
                    green = aimedSpear ? 0 : 255;
                    blue = 125;
                    break;
                default:
                    break;
            }
            Color baseColor = new Color(red, green, blue, 255);

            Color halfBaseColor = baseColor * 0.5f;
            halfBaseColor.A = 0;
            Vector2 projDirection = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Vector2 halfTextureSize = drawTexture.Size() / 2f;
            Color halfOfHalfBaseColor = halfBaseColor * 0.5f;
            float timeLeftColorScale = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 timeLeftDrawEffect = new Vector2(1f, 1.5f) * timeLeftColorScale;
            Vector2 timeLeftDrawEffect2 = new Vector2(0.5f, 1f) * timeLeftColorScale;
            halfBaseColor *= timeLeftColorScale;
            halfOfHalfBaseColor *= timeLeftColorScale;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + projDirection;
                    Color baseColorAlpha = Projectile.GetAlpha(halfBaseColor) * ((Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(drawTexture, drawPos, null, baseColorAlpha, Projectile.rotation, halfTextureSize, timeLeftDrawEffect, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(drawTexture, drawPos, null, baseColorAlpha, Projectile.rotation, halfTextureSize, timeLeftDrawEffect2, SpriteEffects.None, 0);

                    baseColorAlpha = Projectile.GetAlpha(halfOfHalfBaseColor) * ((Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(drawTexture, drawPos, null, baseColorAlpha, Projectile.rotation, halfTextureSize, timeLeftDrawEffect * 0.6f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(drawTexture, drawPos, null, baseColorAlpha, Projectile.rotation, halfTextureSize, timeLeftDrawEffect2 * 0.6f, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(drawTexture, projDirection, null, halfBaseColor, Projectile.rotation, halfTextureSize, timeLeftDrawEffect, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, projDirection, null, halfBaseColor, Projectile.rotation, halfTextureSize, timeLeftDrawEffect2, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, projDirection, null, halfOfHalfBaseColor, Projectile.rotation, halfTextureSize, timeLeftDrawEffect * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, projDirection, null, halfOfHalfBaseColor, Projectile.rotation, halfTextureSize, timeLeftDrawEffect2 * 0.6f, spriteEffects, 0);

            return false;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            //In GFB, "real damage" is replaced with negative healing
            if (Projectile.maxPenetrate >= (int)Providence.BossMode.Red)
                modifiers.SourceDamage *= 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if ((info.Damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red) || target.creativeGodMode)
                return;

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 120, 20);
        }
    }
}

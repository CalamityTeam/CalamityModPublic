using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class SmokingCometYoyo : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 15f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 240f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 14f;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Normal yoyo rotation is +0.5f, so reduce it by 0.25 to compensate for the extra updates
            Projectile.rotation -= 0.25f;

            // Kill if the yoyo is greater than 200 blocks away from the player
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f)
                Projectile.Kill();

            // Rain Starfury stars every 30 frames
            Projectile.localAI[1]++;
            float starRainGateValue = 30f;
            if (Projectile.localAI[1] % starRainGateValue == 0f)
            {
                Vector2 starSpawnLocation = Projectile.Center + new Vector2(Main.rand.Next(-200, 201), -600f);
                Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), starSpawnLocation, Vector2.Normalize(Projectile.Center - starSpawnLocation) * 12f, ProjectileID.Starfury, Projectile.damage, Projectile.knockBack, Projectile.owner);
                star.MaxUpdates = 2;
                star.usesLocalNPCImmunity = true;
                star.localNPCHitCooldown = 40;
            }

            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.Center + new Vector2(-25f, -25f), 50, 50, 58, 0f, 0f, 150, default(Color), 1.2f);

            if (Main.rand.NextBool(10))
                Gore.NewGore(Projectile.GetSource_FromAI(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(16, 18));
        }

        // Hitbox is larger than normal while trying to hit NPCs
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 28f, targetHitbox);

        // Draw the new string
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 vector = Main.player[Projectile.owner].MountedCenter;
            vector.Y += Main.player[Projectile.owner].gfxOffY;
            float num2 = Projectile.Center.X - vector.X;
            float num3 = Projectile.Center.Y - vector.Y;
            Math.Sqrt(num2 * num2 + num3 * num3);
            if (!Projectile.counterweight)
            {
                int num5 = -1;
                if (Projectile.position.X + (float)(Projectile.width / 2) < Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2))
                    num5 = 1;

                num5 *= -1;
                Main.player[Projectile.owner].itemRotation = (float)Math.Atan2(num3 * (float)num5, num2 * (float)num5);
            }

            bool flag = true;
            if (num2 == 0f && num3 == 0f)
            {
                flag = false;
            }
            else
            {
                float num6 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num6 = 12f / num6;
                num2 *= num6;
                num3 *= num6;
                vector.X -= num2 * 0.1f;
                vector.Y -= num3 * 0.1f;
                num2 = Projectile.position.X + (float)Projectile.width * 0.5f - vector.X;
                num3 = Projectile.position.Y + (float)Projectile.height * 0.5f - vector.Y;
            }

            while (flag)
            {
                float num7 = 12f;
                float num8 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                float num9 = num8;
                if (float.IsNaN(num8) || float.IsNaN(num9))
                {
                    flag = false;
                    continue;
                }

                if (num8 < 20f)
                {
                    num7 = num8 - 8f;
                    flag = false;
                }

                num8 = 12f / num8;
                num2 *= num8;
                num3 *= num8;
                vector.X += num2;
                vector.Y += num3;
                num2 = Projectile.position.X + (float)Projectile.width * 0.5f - vector.X;
                num3 = Projectile.position.Y + (float)Projectile.height * 0.1f - vector.Y;
                if (num9 > 12f)
                {
                    float num10 = 0.3f;
                    float num11 = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
                    if (num11 > 16f)
                        num11 = 16f;

                    num11 = 1f - num11 / 16f;
                    num10 *= num11;
                    num11 = num9 / 80f;
                    if (num11 > 1f)
                        num11 = 1f;

                    num10 *= num11;
                    if (num10 < 0f)
                        num10 = 0f;

                    num10 *= num11;
                    num10 *= 0.5f;
                    if (num3 > 0f)
                    {
                        num3 *= 1f + num10;
                        num2 *= 1f - num10;
                    }
                    else
                    {
                        num11 = Math.Abs(Projectile.velocity.X) / 3f;
                        if (num11 > 1f)
                            num11 = 1f;

                        num11 -= 0.5f;
                        num10 *= num11;
                        if (num10 > 0f)
                            num10 *= 2f;

                        num3 *= 1f + num10;
                        num2 *= 1f - num10;
                    }
                }

                float num4 = (float)Math.Atan2(num3, num2) - MathHelper.PiOver2;
                Texture2D stringTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Value;
                Main.spriteBatch.Draw(stringTexture, new Vector2(vector.X - Main.screenPosition.X + (float)ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Width() * 0.5f, vector.Y - Main.screenPosition.Y + (float)ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Height() * 0.5f) - new Vector2(6f, 0f), new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Width(), (int)num7), Color.White, num4, new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Width() * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

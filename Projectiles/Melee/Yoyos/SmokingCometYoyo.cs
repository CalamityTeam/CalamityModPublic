using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class SmokingCometYoyo : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<SmokingComet>();

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
            float yoyoXVel = Projectile.Center.X - vector.X;
            float yoyoYVel = Projectile.Center.Y - vector.Y;
            Math.Sqrt(yoyoXVel * yoyoXVel + yoyoYVel * yoyoYVel);
            if (!Projectile.counterweight)
            {
                int distanceCheck = -1;
                if (Projectile.position.X + (float)(Projectile.width / 2) < Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2))
                    distanceCheck = 1;

                distanceCheck *= -1;
                Main.player[Projectile.owner].itemRotation = (float)Math.Atan2(yoyoYVel * (float)distanceCheck, yoyoXVel * (float)distanceCheck);
            }

            bool isActive = true;
            if (yoyoXVel == 0f && yoyoYVel == 0f)
            {
                isActive = false;
            }
            else
            {
                float yoyoVelocity = (float)Math.Sqrt(yoyoXVel * yoyoXVel + yoyoYVel * yoyoYVel);
                yoyoVelocity = 12f / yoyoVelocity;
                yoyoXVel *= yoyoVelocity;
                yoyoYVel *= yoyoVelocity;
                vector.X -= yoyoXVel * 0.1f;
                vector.Y -= yoyoYVel * 0.1f;
                yoyoXVel = Projectile.position.X + (float)Projectile.width * 0.5f - vector.X;
                yoyoYVel = Projectile.position.Y + (float)Projectile.height * 0.5f - vector.Y;
            }

            while (isActive)
            {
                float chainWidth = 12f;
                float yoyoVelocityAgain = (float)Math.Sqrt(yoyoXVel * yoyoXVel + yoyoYVel * yoyoYVel);
                float yoyoVelocityCopy = yoyoVelocityAgain;
                if (float.IsNaN(yoyoVelocityAgain) || float.IsNaN(yoyoVelocityCopy))
                {
                    isActive = false;
                    continue;
                }

                if (yoyoVelocityAgain < 20f)
                {
                    chainWidth = yoyoVelocityAgain - 8f;
                    isActive = false;
                }

                yoyoVelocityAgain = 12f / yoyoVelocityAgain;
                yoyoXVel *= yoyoVelocityAgain;
                yoyoYVel *= yoyoVelocityAgain;
                vector.X += yoyoXVel;
                vector.Y += yoyoYVel;
                yoyoXVel = Projectile.position.X + (float)Projectile.width * 0.5f - vector.X;
                yoyoYVel = Projectile.position.Y + (float)Projectile.height * 0.1f - vector.Y;
                if (yoyoVelocityCopy > 12f)
                {
                    float absVelocityCheck = 0.3f;
                    float absVelocity = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
                    if (absVelocity > 16f)
                        absVelocity = 16f;

                    absVelocity = 1f - absVelocity / 16f;
                    absVelocityCheck *= absVelocity;
                    absVelocity = yoyoVelocityCopy / 80f;
                    if (absVelocity > 1f)
                        absVelocity = 1f;

                    absVelocityCheck *= absVelocity;
                    if (absVelocityCheck < 0f)
                        absVelocityCheck = 0f;

                    absVelocityCheck *= absVelocity;
                    absVelocityCheck *= 0.5f;
                    if (yoyoYVel > 0f)
                    {
                        yoyoYVel *= 1f + absVelocityCheck;
                        yoyoXVel *= 1f - absVelocityCheck;
                    }
                    else
                    {
                        absVelocity = Math.Abs(Projectile.velocity.X) / 3f;
                        if (absVelocity > 1f)
                            absVelocity = 1f;

                        absVelocity -= 0.5f;
                        absVelocityCheck *= absVelocity;
                        if (absVelocityCheck > 0f)
                            absVelocityCheck *= 2f;

                        yoyoYVel *= 1f + absVelocityCheck;
                        yoyoXVel *= 1f - absVelocityCheck;
                    }
                }

                float stringHelper = (float)Math.Atan2(yoyoYVel, yoyoXVel) - MathHelper.PiOver2;
                Texture2D stringTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Value;
                Main.spriteBatch.Draw(stringTexture, new Vector2(vector.X - Main.screenPosition.X + (float)ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Width() * 0.5f, vector.Y - Main.screenPosition.Y + (float)ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Height() * 0.5f) - new Vector2(6f, 0f), new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Width(), (int)chainWidth), Color.White, stringHelper, new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SmokingCometChain").Width() * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}

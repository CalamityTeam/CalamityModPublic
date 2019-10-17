using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles
{
    public class SlickCaneProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slick Cane");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 36;
            projectile.Calamity().rogue = true;
            projectile.timeLeft = 90;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.hide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
            projectile.alpha = 180;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.Center;
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            projectile.Center = vector;
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            if (!player.frozen)
            {
                projectile.spriteDirection = projectile.direction = player.direction;
                projectile.alpha -= 127;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                if (projectile.localAI[0] > 0f)
                {
                    projectile.localAI[0] -= 1f;
                }
                float itemAnimationRatio = (float)player.itemAnimation / (float)player.itemAnimationMax;
                float inverseItemAnimationRatio = 1f - itemAnimationRatio;
                float rotatedVelocity = projectile.velocity.ToRotation();
                float velocityMagnitude = projectile.velocity.Length();
                float additive = 22f;
                Vector2 spinningpoint = Vector2.UnitX.RotatedBy((double)(MathHelper.Pi + inverseItemAnimationRatio * MathHelper.TwoPi), default) * new Vector2(velocityMagnitude, projectile.ai[0]) * 0.7f;
                projectile.position += spinningpoint.RotatedBy((double)rotatedVelocity, default) + new Vector2(velocityMagnitude + additive, 0f).RotatedBy((double)rotatedVelocity, default) / 15f;
                Vector2 destination = vector + spinningpoint.RotatedBy((double)rotatedVelocity, default) + new Vector2(velocityMagnitude + additive + 40f, 0f).RotatedBy((double)rotatedVelocity, default) / 1.8f;
                projectile.rotation = player.AngleTo(destination) + ((float)(Math.PI * 0.25)) * (float)player.direction; //or this
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation += (float)Math.PI; //change this
                }
                player.DirectionTo(projectile.Center);
            }
            if (player.itemAnimation == 2)
            {
                projectile.Kill();
                player.reuseDelay = 2;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            Vector2 vector53 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D31 = (projectile.spriteDirection == -1) ? ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SlickCaneProjectileAlt") : Main.projectileTexture[projectile.type];
            Color alpha4 = projectile.GetAlpha(color25);
            Vector2 origin8 = new Vector2((float)texture2D31.Width, (float)texture2D31.Height) / 2f;
            origin8 = new Vector2((projectile.spriteDirection == 1) ? ((float)texture2D31.Width - -8f) : -8f, -8f); //-8 -8
            SpriteBatch arg_E055_0 = Main.spriteBatch;
            Vector2 arg_E055_2 = vector53;
            Rectangle? sourceRectangle2 = null;
            arg_E055_0.Draw(texture2D31, arg_E055_2, sourceRectangle2, new Color(255, 255, 255, 127), projectile.rotation, origin8, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float f2 = projectile.rotation - 0.7853982f * (float)Math.Sign(projectile.velocity.X) + ((projectile.spriteDirection == -1) ? 3.14159274f : 0f);
            float velocityMagnitude = 0f;
            float scaleFactor = -95f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + f2.ToRotationVector2() * scaleFactor, 23f * projectile.scale, ref velocityMagnitude))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float moneyValueToDrop = target.value / Main.rand.NextFloat(15f, 35f);
            if (projectile.Calamity().stealthStrike && Main.rand.NextBool(15))
            {
                moneyValueToDrop += Item.buyPrice(0, Main.rand.Next(1, 4), Main.rand.Next(0, 100), Main.rand.Next(0, 100));
            }
            if (moneyValueToDrop > 1000000f)
            {
                int modifiedMoneyValue = (int)(moneyValueToDrop / 1000000f);
                if (modifiedMoneyValue > 50 && Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                if (Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                moneyValueToDrop -= (float)(1000000 * modifiedMoneyValue);
                Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, 74, modifiedMoneyValue, false, 0, false, false);
            }
            else if (moneyValueToDrop > 10000f)
            {
                int modifiedMoneyValue = (int)(moneyValueToDrop / 10000f);
                if (modifiedMoneyValue > 50 && Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                if (Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                moneyValueToDrop -= (float)(10000 * modifiedMoneyValue);
                Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, 73, modifiedMoneyValue, false, 0, false, false);
            }
            else if (moneyValueToDrop > 100f)
            {
                int modifiedMoneyValue = (int)(moneyValueToDrop / 100f);
                if (modifiedMoneyValue > 50 && Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                if (Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                moneyValueToDrop -= (float)(100 * modifiedMoneyValue);
                Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, 72, modifiedMoneyValue, false, 0, false, false);
            }
            else
            {
                int modifiedMoneyValue = (int)moneyValueToDrop;
                if (modifiedMoneyValue > 50 && Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(3) + 1;
                }
                if (Main.rand.Next(5) == 0)
                {
                    modifiedMoneyValue /= Main.rand.Next(4) + 1;
                }
                if (modifiedMoneyValue < 1)
                {
                    modifiedMoneyValue = 1;
                }
                moneyValueToDrop -= (float)modifiedMoneyValue;
                Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, 71, modifiedMoneyValue, false, 0, false, false);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Rogue
{
    public class SlickCaneProjectile : BaseSpearProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SlickCane";

        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slick Cane");
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 36;
            Projectile.Calamity().rogue = true;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.alpha = 180;
        }
        public override SpearType SpearAiType => SpearType.GhastlyGlaiveSpear;
        public override float TravelSpeed => 8f;

        public override bool PreAI()
        {
            // Initialization. Using the AI hook would override the base spear's code, and we don't want that.
            if (!initialized)
            {
                Main.player[Projectile.owner].Calamity().ConsumeStealthByAttacking();
                initialized = true;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D alternateHookTexture = Projectile.spriteDirection == -1 ? ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SlickCaneProjectileAlt") : ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(Projectile.spriteDirection == 1 ? alternateHookTexture.Width + 8f : -8f, -8f);
            spriteBatch.Draw(alternateHookTexture, drawPosition, null,
                lightColor, Projectile.rotation,
                origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float f2 = Projectile.rotation - MathHelper.PiOver4 *
                Math.Sign(Projectile.velocity.X) + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            float velocityMagnitude = 0f;
            float scaleFactor = 35f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + f2.ToRotationVector2() * scaleFactor,
                (TravelSpeed + 1f) * Projectile.scale, ref velocityMagnitude))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                float moneyValueToDrop = target.value / Main.rand.NextFloat(15f, 35f);
                // Maximum of 50 silver, not counting steath strikes
                moneyValueToDrop = (int)MathHelper.Clamp(moneyValueToDrop, 0, 5000f);
                if (Projectile.Calamity().stealthStrike && Main.rand.NextBool(15))
                {
                    moneyValueToDrop += Item.buyPrice(0, Main.rand.Next(1, 4), Main.rand.Next(0, 100), Main.rand.Next(0, 100));
                }

                while (moneyValueToDrop > 10000f)
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
                    moneyValueToDrop -= 10000 * modifiedMoneyValue;
                    DropHelper.DropItem(target, ItemID.GoldCoin, modifiedMoneyValue);
                }
                while (moneyValueToDrop > 100f)
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
                    moneyValueToDrop -= 100 * modifiedMoneyValue;
                    DropHelper.DropItem(target, ItemID.SilverCoin, modifiedMoneyValue);
                }
                while (moneyValueToDrop > 0f)
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
                    moneyValueToDrop -= modifiedMoneyValue;
                    DropHelper.DropItem(target, ItemID.CopperCoin, modifiedMoneyValue);
                }
            }
        }
    }
}

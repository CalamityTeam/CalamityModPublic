using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SlickCaneProjectile : BaseSpearProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<SlickCane>();
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SlickCane";

        private bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 36;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 180;
            Projectile.scale = 1.25f;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D alternateHookTexture = Projectile.spriteDirection == -1 ? ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SlickCaneProjectileAlt").Value : Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 origin = new Vector2(Projectile.spriteDirection == 1 ? alternateHookTexture.Width + 8f : -8f, -8f);
            Main.EntitySpriteDraw(alternateHookTexture, drawPosition, null,
                lightColor, Projectile.rotation,
                origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float spearLengthMult = 2.5f;
            float velocityMagnitude = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Main.player[Projectile.owner].Center, Main.player[Projectile.owner].Center + Projectile.velocity * spearLengthMult,
                (TravelSpeed + 1f) * Projectile.scale, ref velocityMagnitude))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            //Dont drop money from Dummies, WoF eyes and worm segments to make grinding money a bit harder and not give money from Dummies since that was apparently happening
            if (Projectile.owner == Main.myPlayer && target.IsAnEnemy(false) && !target.dontCountMe && !CalamityNPCSets.ForceDrawDebuffDisplay[target.type])
            {
                float moneyValueToDrop = target.value / Main.rand.NextFloat(15f, 35f);
                // Maximum of 50 silver, not counting steath strikes
                moneyValueToDrop = (int)MathHelper.Clamp(moneyValueToDrop, 0, 5000f);
                if (Projectile.Calamity().stealthStrike && Main.rand.NextBool(20))
                {
                    moneyValueToDrop += Item.buyPrice(0, Main.rand.Next(1, 3), Main.rand.Next(0, 100), Main.rand.Next(0, 100));
                    SoundEngine.PlaySound(TheSevensStriker.TriplesSound, Projectile.Center);
                }
                else if (moneyValueToDrop > 0f)
                    SoundEngine.PlaySound(TheSevensStriker.DoublesSound, Projectile.Center);
                while (moneyValueToDrop > 10000f)
                {
                    int modifiedMoneyValue = (int)(moneyValueToDrop / 10000f);
                    if (modifiedMoneyValue > 50 && Main.rand.NextBool(5))
                    {
                        modifiedMoneyValue /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        modifiedMoneyValue /= Main.rand.Next(3) + 1;
                    }
                    moneyValueToDrop -= 10000 * modifiedMoneyValue;
                    Item.NewItem(target.GetSource_Loot(), target.Hitbox, ItemID.GoldCoin, modifiedMoneyValue);
                }
                while (moneyValueToDrop > 100f)
                {
                    int modifiedMoneyValue = (int)(moneyValueToDrop / 100f);
                    if (modifiedMoneyValue > 50 && Main.rand.NextBool(5))
                    {
                        modifiedMoneyValue /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        modifiedMoneyValue /= Main.rand.Next(3) + 1;
                    }
                    moneyValueToDrop -= 100 * modifiedMoneyValue;
                    Item.NewItem(target.GetSource_Loot(), target.Hitbox, ItemID.SilverCoin, modifiedMoneyValue);
                }
                while (moneyValueToDrop > 0f)
                {
                    int modifiedMoneyValue = (int)moneyValueToDrop;
                    if (modifiedMoneyValue > 50 && Main.rand.NextBool(5))
                    {
                        modifiedMoneyValue /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        modifiedMoneyValue /= Main.rand.Next(4) + 1;
                    }
                    if (modifiedMoneyValue < 1)
                    {
                        modifiedMoneyValue = 1;
                    }
                    moneyValueToDrop -= modifiedMoneyValue;
                    Item.NewItem(target.GetSource_Loot(), target.Hitbox, ItemID.CopperCoin, modifiedMoneyValue);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Boost dmg in proportion to money, caps at 1 plat.
            if (Projectile.Calamity().stealthStrike)
            {
                Player player = Main.player[Projectile.owner];
                double money = Utils.CoinsCount(out bool overflow, player.inventory);
                double cap = 1000000;
                if (money >= cap)
                    money = cap;
                if (money != 0)
                {
                    modifiers.SourceDamage *= (float)(money / 1000000 + 1);
                    SoundEngine.PlaySound(TheSevensStriker.JackpotSound, Projectile.Center);
                    for (int j = 0; j < 8; j++)
                    {
                        int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 3f);
                        Main.dust[dust2].noGravity = true;
                        Main.dust[dust2].velocity *= 5f;
                        dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 100, default, 2f);
                        Main.dust[dust2].velocity *= 2f;
                    }
                }
            }
        }
    }
}

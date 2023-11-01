using System;
using System.Collections.Generic;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MidasPrime : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        internal static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/CrackshotColtShot") { Volume = 0.5f, PitchVariance = 0.1f };

        // Internal storage used to keep track between UseItem and Shoot hooks whether a gold coin was queued up
        private bool nextShotGoldCoin = false;

        public override void SetDefaults()
        {
            Item.damage = 81;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 23;
            Item.height = 8;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = ShootSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MarksmanShot>();
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 14f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // This item has a right click.
        public override bool AltFunctionUse(Player player) => true;

        // This item enables the automatic syncing of player mouse coordinates while held.
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        // This item never uses ammo when right clicking.
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse != 2;

        public override bool CanUseItem(Player player)
        {
            // Two things are checked for right click:
            // 1) The player has at least 1 silver coin to toss
            // 2) The player doesn't have 4 ricoshot coins (of any type) in the air already
            if (player.altFunctionUse == 2)
            {
                // player.CanBuyItem() breaks if the player has more than 2,147 platinum coins and was never fixed
                // This alternative method works no matter how much money the player has
                long cashAvailable = Utils.CoinsCount(out bool overflow, player.inventory);
                if (cashAvailable < 100 && !overflow)
                    return false;

                return player.GetActiveRicoshotCoinCount() < 4;
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            // Remove either 1 gold (if possible) or 1 silver (otherwise) when using right click
            if (player.altFunctionUse == 2)
            {
                long cashAvailable = Utils.CoinsCount(out bool overflow, player.inventory);

                // If the player has at least 1 gold in their inventory, spend it and use a gold coin
                if (overflow || cashAvailable > 10000)
                {
                    player.BuyItem(10000);
                    nextShotGoldCoin = true;
                }

                // Otherwise, spend 1 silver and use a silver coin
                else
                {
                    player.BuyItem(100);
                    nextShotGoldCoin = false;
                }
            }

            return base.UseItem(player);
        }

        // This hook is a convenient location to change the use sound.
        public override void UseAnimation(Player player)
        {
            Item.UseSound = ShootSound; 
            if (player.altFunctionUse == 2)
                Item.UseSound = RicoshotCoin.BlingSound;
        }

        // Coins can be tossed much faster than bullets can be fired.
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 3f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Move all fired projectiles 15 pixels upwards so they don't come out of the player's groin
            position -= Vector2.UnitY * 15f;

            // No matter what type of ammo is used, left click will fire a Marksman Round
            type = ModContent.ProjectileType<MarksmanShot>();

            // Right clicks toss coins instead
            if (player.altFunctionUse == 2)
            {
                damage = 0;
                type = ModContent.ProjectileType<RicoshotCoin>();
                velocity = player.GetCoinTossVelocity();
            }
        }

        // This shoot override is only needed to set ai[0] to be either 1f or 2f for silver and gold coins.
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                float coinAIVariable = nextShotGoldCoin ? 2f : 1f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, coinAIVariable);
                return false;
            }

            // Otherwise use default behavior (which is just to return true).
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        #region Hidden ULTRAKILL Reference Tooltip
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine ultrakillIntro = new TooltipLine(Mod, "CalamityMod:UltrakillIntroReference", this.GetLocalizedValue("UltrakillEasterEgg")) { OverrideColor = Color.Red };
            CalamityUtils.HoldShiftTooltip(tooltips, new TooltipLine[] { ultrakillIntro });
        }
        #endregion

        // Make the gun have visible recoil when fired for extra cool factor.
        #region Firing Animation
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(50, 24);
            Vector2 itemOrigin = new Vector2(-17, 3);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.45f * (float)Math.Pow((0.4f - animProgress) / 0.4f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        #endregion
    }
}

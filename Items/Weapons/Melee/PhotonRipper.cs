using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using CalamityMod.Systems.Collections;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PhotonRipper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<MiracleBlight>()];
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 134;
            Item.height = 54;
            Item.damage = 1400;
            Item.crit = 18;
            Item.knockBack = 12f;
            Item.useTime = 5;
            Item.useAnimation = 25;
            // In-game, the displayed axe power is 5x the value set here.
            // This corrects for trees having 500% hardness internally.
            // So that the axe power in the code looks like the axe power you see on screen, divide by 5.
            Item.axe = 3000 / 5;
            // Photon Ripper's axe power is entirely for show. Its projectiles instantly one shot trees.

            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<PhotonRipperProjectile>();
            Item.shootSpeed = 1f;

            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool AltFunctionUse(Player player) => true;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float breakBlocks = 1;
            // If right clicking, the chainsaw won't be able to chop down trees
            if (player.Calamity().mouseRight && player.whoAmI == Main.myPlayer && !Main.mapFullscreen && !Main.blockMouse)
            {
                breakBlocks = 0;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f, breakBlocks);
            return false;
        }
    }
}

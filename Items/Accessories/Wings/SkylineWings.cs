using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class SkylineWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyline Wings");
            Tooltip.SetDefault("Horizontal speed: 6.25\n" +
                "Acceleration multiplier: 1.0\n" +
                "Average vertical speed\n" +
                "Flight time: 80\n" +
                "10% increased jump speed while wearing the Aerospec armor");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<AerospecHat>() || player.armor[0].type == ModContent.ItemType<AerospecHeadgear>() ||
                player.armor[0].type == ModContent.ItemType<AerospecHelm>() || player.armor[0].type == ModContent.ItemType<AerospecHood>() ||
                player.armor[0].type == ModContent.ItemType<AerospecHelmet>()) &&
                player.armor[1].type == ModContent.ItemType<AerospecBreastplate>() && player.armor[2].type == ModContent.ItemType<AerospecLeggings>())
            {
                player.jumpSpeedBoost += 0.5f;
            }
            player.wingTimeMax = 80;
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 6.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AerialiteBar>(5)
                .AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

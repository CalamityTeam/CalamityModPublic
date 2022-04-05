using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CrimsonFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Flask");
            Tooltip.SetDefault("7% increased damage reduction and +3 defense while in the crimson");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneCrimson)
            {
                player.statDefense += 6;
                player.endurance += 0.07f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ViciousPowder, 15)
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

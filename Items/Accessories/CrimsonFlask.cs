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
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneCrimson)
            {
                player.statDefense += 3;
                player.endurance += 0.07f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.VisciousPowder, 15);
            recipe.AddIngredient(ItemID.Vertebrae, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

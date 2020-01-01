using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class FearmongerGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Greaves");
            Tooltip.SetDefault(@"15% increased damage reduction
Taking damage causes the player to panic");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.defense = 48;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.1f;
			player.panic = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 13);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 6);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 6);
            recipe.AddIngredient(ItemID.SoulofFright, 9);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
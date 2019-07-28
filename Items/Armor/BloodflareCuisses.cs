using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class BloodflareCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Cuisses");
            Tooltip.SetDefault("30% increased movement speed, 10% increased damage and 7% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 36, 0, 0);
			item.defense = 29;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.3f;
			player.allDamage += 0.1f;
			player.GetModPlayer<CalamityPlayer>().AllCritBoost(7);
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 13);
            recipe.AddIngredient(null, "RuinousSoul", 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

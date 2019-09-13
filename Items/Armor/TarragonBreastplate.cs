using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class TarragonBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Breastplate");
            Tooltip.SetDefault("10% increased damage and 5% increased critical strike chance\n" +
                       "+2 life regen and +40 max life\n" +
                       "Breastplate of the exiler");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.lifeRegen = 2;
			item.value = Item.buyPrice(0, 40, 0, 0);
			item.defense = 37;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
			player.allDamage += 0.1f;
			player.GetModPlayer<CalamityPlayer>().AllCritBoost(5);
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar", 15);
            recipe.AddIngredient(null, "DivineGeode", 18);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

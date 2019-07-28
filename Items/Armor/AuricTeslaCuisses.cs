using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AuricTeslaCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Cuisses");
            Tooltip.SetDefault("50% increased movement speed\n" +
                "12% increased damage and 5% increased critical strike chance\n" +
                "Magic carpet effect");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(1, 8, 0, 0);
			item.defense = 44;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.5f;
            player.carpet = true;
			player.allDamage += 0.12f;
			player.GetModPlayer<CalamityPlayer>().AllCritBoost(5);
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaLeggings");
            recipe.AddIngredient(null, "GodSlayerLeggings");
            recipe.AddIngredient(null, "BloodflareCuisses");
            recipe.AddIngredient(null, "TarragonLeggings");
            recipe.AddIngredient(null, "AuricOre", 80);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "Phantoplasm", 15);
            recipe.AddIngredient(null, "DarksunFragment", 10);
            recipe.AddIngredient(null, "BarofLife", 8);
            recipe.AddIngredient(null, "HellcasterFragment", 6);
            recipe.AddIngredient(null, "CoreofCalamity", 3);
            recipe.AddIngredient(null, "GalacticaSingularity", 2);
            recipe.AddIngredient(ItemID.FlyingCarpet);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

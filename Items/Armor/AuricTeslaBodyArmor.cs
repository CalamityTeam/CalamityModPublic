using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class AuricTeslaBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Body Armor");
            Tooltip.SetDefault("+100 max life\n" +
                       "25% increased movement speed\n" +
                       "Attacks have a 2% chance to do no damage to you\n" +
                       "8% increased damage and 5% increased critical strike chance\n" +
                       "You will freeze enemies near you when you are struck");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(1, 44, 0, 0);
			item.defense = 48;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.fBarrier = true;
            modPlayer.godSlayerReflect = true;
            player.statLifeMax2 += 100;
            player.moveSpeed += 0.25f;
			player.allDamage += 0.08f;
			modPlayer.AllCritBoost(5);
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaArmor");
            recipe.AddIngredient(null, "GodSlayerChestplate");
            recipe.AddIngredient(null, "BloodflareBodyArmor");
            recipe.AddIngredient(null, "TarragonBreastplate");
            recipe.AddIngredient(null, "AuricOre", 100);
            recipe.AddIngredient(null, "EndothermicEnergy", 30);
            recipe.AddIngredient(null, "NightmareFuel", 30);
            recipe.AddIngredient(null, "Phantoplasm", 20);
            recipe.AddIngredient(null, "DarksunFragment", 15);
            recipe.AddIngredient(null, "BarofLife", 10);
            recipe.AddIngredient(null, "HellcasterFragment", 7);
            recipe.AddIngredient(null, "CoreofCalamity", 5);
            recipe.AddIngredient(null, "GalacticaSingularity", 3);
            recipe.AddIngredient(null, "FrostBarrier");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Nincity
    public class AngelicAlliance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angelic Alliance");
            Tooltip.SetDefault("Increases all damage by 10%\n" +
			"Add some extra tooltip shit\n" +
			"Something something hotkey");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 96;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.accessory = true;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Developer;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AngelicAllianceHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip2")
                {
                    line2.text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a short range teleport and render you momentarily invulnerable";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.angelicAlliance = true;
			player.allDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            PSCRecipe recipe = new PSCRecipe(mod);
            recipe.AddRecipeGroup("AnyHallowedHelmet");
            //recipe.AddRecipeGroup("AnyHallowedPlatemail");
            //recipe.AddRecipeGroup("AnyHallowedGreaves");
            recipe.AddIngredient(ItemID.HallowedPlateMail);
            recipe.AddIngredient(ItemID.HallowedGreaves);
            recipe.AddIngredient(ModContent.ItemType<ElysianAegis>());
            recipe.AddIngredient(ItemID.Excalibur);
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

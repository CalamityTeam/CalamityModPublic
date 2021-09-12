using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmbrosialAmpoule : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ambrosial Ampoule");
            Tooltip.SetDefault("You emit light\n" +
                "7% increased damage reduction and increased life regen\n" +
                "Freeze, chill and frostburn immunity");
        }

        public override void SetDefaults()
        {
            item.defense = 6;
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip2")
					{
						line2.text = "Freeze, chill and frostburn immunity\n" +
						"Provides cold protection in Death Mode";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aAmpoule = true;
			modPlayer.rOoze = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CorruptFlask>());
            recipe.AddIngredient(ModContent.ItemType<RadiantOoze>());
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CrimsonFlask>());
            recipe.AddIngredient(ModContent.ItemType<RadiantOoze>());
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

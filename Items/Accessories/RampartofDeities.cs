using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RampartofDeities : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rampart of Deities");
            Tooltip.SetDefault("Taking damage grants boosted movement speed for a short time\n" +
                "Causes stars to fall and gives increased immune time when damaged\n" +
                "Increases armor penetration by 20 and reduces the cooldown of healing potions\n" +
                "Provides light underwater and provides a small amount of light in the abyss\n" +
                "Absorbs 25% of damage done to players on your team\n" +
                "This effect is only active above 25% life\n" +
                "Grants immunity to knockback\n" +
                "Puts a shell around the owner when below 50% life that reduces damage\n" +
                "The shell becomes more powerful when below 15% life and reduces damage even further\n" +
				"Provides heat and cold protection in Death Mode");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.defense = 12;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (!CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip9")
					{
						line2.text = "";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dAmulet = true;
            modPlayer.rampartOfDeities = true;
            modPlayer.fBulwark = true;
            modPlayer.jellyfishNecklace = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrigidBulwark>());
            recipe.AddIngredient(ModContent.ItemType<DeificAmulet>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

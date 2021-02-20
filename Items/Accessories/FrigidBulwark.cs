using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FrigidBulwark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigid Bulwark");
            Tooltip.SetDefault("Absorbs 25% of damage done to players on your team\n" +
                "Only active above 25% life\n" +
                "Grants immunity to knockback\n" +
                "Puts a shell around the owner when below 50% life that reduces damage\n" +
                "The shell becomes more powerful when below 15% life and reduces damage even further");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.defense = 8;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
					{
						line2.text = "The shell becomes more powerful when below 15% life and reduces damage even further\n" +
						"Provides heat and cold protection in Death Mode";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fBulwark = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PaladinsShield);
            recipe.AddIngredient(ItemID.FrozenTurtleShell);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

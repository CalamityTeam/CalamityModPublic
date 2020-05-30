using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles;

namespace CalamityMod.Items.Materials
{
    public class CruptixBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scoria Bar");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
            Tooltip.SetDefault("The smoke feels warm\n" +
				"Looks funky when placed"); //explaining the texture shown when placed.  same bug as red lightning container and ectoheart tbh
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<ChaoticBarPlaced>();
			item.noUseGraphic = true; //no use graphic because animated things swing their entire sheet
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 30;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 1, silver: 20);
            item.rare = 8;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ChaoticOre>(), 5);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

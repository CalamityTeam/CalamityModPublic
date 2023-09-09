using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class AstralBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Type] = 99; // Luminite
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 12));
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AstralBar>());
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(3).
                AddIngredient<AstralOre>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

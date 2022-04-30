using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.SummonItems
{
    public class Starcore : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Starcore");
            Tooltip.SetDefault("May the stars guide your way\n" +
                "Summons Astrum Deus at the Astral Beacon, but is not consumed");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(25).
                AddIngredient<AstralJelly>(8).
                AddIngredient<AstralBar>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePolterghast : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Tooltip.SetDefault("A creature born of hatred and anger, formed by countless human souls with all of their energy entirely devoted to consuming others.\n" +
                "It seems a waste to have had such a potent source of power ravage mindlessly through these empty halls.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PolterghastTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAstrumAureus : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Aureus");
            Tooltip.SetDefault("A titanic cyborg infected by a star-borne disease expelled from the belly of an ancient god.\n" +
                "The destruction of this creature will not prevent the spread of the disease.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstrumAureusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

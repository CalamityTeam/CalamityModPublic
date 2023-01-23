using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeOcean")]
    public class LoreAbyss : LoreItem
    {
        public override string Lore =>
@"While there are many sightless crevasses in the deep sea, this one is a unique geological marvel.
It is located unsettlingly close to the shoreline. Somehow, even eons of tectonics could not seal or crush it.
The isolated Abyss is the debatably tranquil home of the naiad Anahita and some reclusive sea creatures.
Here, I disposed of the burgeoning remains of Silva, the Goddess of Life itself. Obviously, she of all Gods refused to truly die.
My wishes were that she would be forgotten, but her tenacity is remarkable.
Diffused, her influence inundated that pit of crushing pressure with flora and fauna aplenty.
Her great roots continue to thrash and tear at the impossibly dense stone, growing uncontrollably.
She will soon remake it in her image. I can think of no worse fate for this accursed, benthic domain.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Abyss");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LeviathanTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<AnahitaTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

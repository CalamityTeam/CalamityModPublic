using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePlaguebringerGoliath")]
    public class LorePlaguebringerGoliath : LoreItem
    {
        public override string Lore =>
@"An innocent queen, forced to bear an agonizing existence. This is nothing short of a crime against nature.
Without consulting me, Draedon sought to weaponize the already well-organized Jungle bees.
When he revealed his finished project, I was enraged. Enslaving the bees was despicable.
Draedon cared little for my outrage and returned to his other work without further incident.
From that point on, I stopped making requests of Draedon. He had shown me his true colors.
In my later days I was far from virtuous. But I would not shackle a creature to fight in my name.
That would make me no better than the divine scoundrels I pursued.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Plaguebringer Goliath");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlaguebringerGoliathTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

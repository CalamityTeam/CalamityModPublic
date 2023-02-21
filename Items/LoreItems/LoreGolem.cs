using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeGolem")]
    public class LoreGolem : LoreItem
    {
        public override string Lore =>
@"What a sad, piteous thing. Truly, a mockery in every sense of the word.
The Lihzahrds were abandoned by their deity long ago. They set upon creating the idol as a replacement.
The result is an amalgamation of the concepts and themes of many Gods, most prominently the heat of the sun.
It is a far cry from a mechanical god… for the better. The alternative is too chilling to consider.
While I believe it barely deserves mention, the Lihzahrds revere it unflinchingly.
I see no need to intervene in affairs beneath me and my people.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Golem");
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
                AddIngredient(ItemID.GolemTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

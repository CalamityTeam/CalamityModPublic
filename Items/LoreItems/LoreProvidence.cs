using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeProvidence")]
    public class LoreProvidence : LoreItem
    {
        public override string Lore =>
@"A glorious day.
Deeds of valor of this caliber are enshrined in legend. Of this age, only the Witch, Braelor and myself can compare.
Providence was perhaps one of the wickedest Gods, hellbent on purification through erasure.
Her worshippers saw little value in life. Pain was not a price they felt justified to pay.
The Profaned Goddess promised her followers she would end inequality by reducing all to featureless ash.
Those devoted to her were weak-willed, and yet she reigned as one of the mightiest Gods.
Perhaps it was their easily-swayed nature, that let her draw so much power from them…";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Providence, the Profaned Goddess");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ProvidenceTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

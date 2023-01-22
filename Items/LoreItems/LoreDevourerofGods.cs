using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDevourerofGods")]
    public class LoreDevourerofGods : LoreItem
    {
        public override string Lore =>
@"The infamous, otherworldly glutton, in the flesh. His imposing title was self-granted, but truth made it stick.
He is a formidable foe, capable of swallowing Gods whole, absorbing their essence in its entirety.
I ordered Draedon to armor his gargantuan form, so he could safely best even great Gods in single combat.
Fittingly, he had a serpent’s tongue. He manipulated me incessantly, driving me to awful acts.
I recruited him out of desperation. My war had dragged on for decades, and I would do anything to have it end.
It was then my negligence was born. My descent began the moment recruiting this scoundrel crossed my mind.
His dearth of loyalty was clear as day, even at the time. However, I suspect it goes beyond that.
The Devourer’s alien capabilities and domineering tactics hint that his allegiance lay elsewhere.
Is he but one soldier, of a malevolence far beyond…?";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DevourerofGodsTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

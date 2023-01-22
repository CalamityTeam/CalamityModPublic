using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeMechs")]
    public class LoreMechs : LoreItem
    {
        public override string Lore =>
@"These unwieldy beasts of steel were the experiments of Draedon, my former colleague and prodigious engineer.
His intent was to fuel a war machine with soul energy, allowing it to fight with purpose and zeal.
The creations were a success; so much so, that the souls continued to express their own free will.
Draedon was displeased. But these were my soldiers, and their loyalty was forged anew in iron.
I dismissed them from duty, and yet here they are, scouring the land for evidence of the divine.
Unfortunately for you, that puts you in their crosshairs. Give them a battle worth dying in.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Mechanical Trio");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RetinazerTrophy).
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SpazmatismTrophy).
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.DestroyerTrophy).
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SkeletronPrimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

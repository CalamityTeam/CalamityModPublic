using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeKingSlime")]
    public class LoreKingSlime : LoreItem
    {
        public override string Lore =>
@"Given time, these gelatinous creatures absorb each other and slowly grow in both size and strength.
There is little need to worry about this. Naturally, slimes are nearly mindless and amass only by chance.
Though it appears they are capable of absorbing knowledge, if only in rudimentary form.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("King Slime");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.KingSlimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

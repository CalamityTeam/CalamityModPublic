using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSkeletron")]
    public class LoreSkeletron : LoreItem
    {
        public override string Lore =>
@"A sorry old man, cursed by an even older cult, caught trespassing on their ancient library.
They were once my friends. Their leader is infatuated with Dragons, and dreams of resurrecting one.
The very walls of that place are cursed further still. The magic has long since faded, and the soldiers rotted.
Do not expect to learn much from those tattered tomes. They were penned with misguided zeal.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Skeletron");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SkeletronTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSkeletronPrime")]
    public class LoreSkeletronPrime : LoreItem
    {
        public override string Lore =>
@"So consumed by hatred were some souls, that they pledged they would do anything in my name.
Their devotion was unerring. Absolute. No atrocity was beyond them; their vengeance knew no bounds.
I organized them into shock troops, dreaded for their flamethrowers and incendiaries.
Leveling places of worship and torching those falsely devout, their expertise lay in unmaking faith with flame.
Draedon understood well. For them he crafted a visage so grim, it evoked oblivion itself.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Skeletron Prime");
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
                AddIngredient(ItemID.SkeletronPrimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

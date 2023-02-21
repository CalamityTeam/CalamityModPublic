using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreEmpressofLight : LoreItem
    {
        public override string Lore =>
@"Though her title is lofty, she is more an emissary for the powers beyond and forces of nature.
In broad daylight, she can channel the Primordial Light itself, making her nigh untouchable.
Thankfully, left with only starlight to wield, she falls like any other graceless despot.
Her penchant for leeching the strength of other great beings is uniquely deplorable.
It made her sickeningly obedient. Dependent, but willingly so, as they enabled her to slake her base thirst.
I had deigned to slay her myself for her treachery, but she was a notoriously evasive mark.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Empress of Light");
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
                AddIngredient(ItemID.FairyQueenTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

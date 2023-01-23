using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeMoonLord")]
    public class LoreRequiem : LoreItem
    {
        public override string Lore =>
@"As the Light Dragon was fading, the monk visited him. Nearly none understand what transpired that day.
Most say his passing was eased. The truth? Zeratros’ Auric soul was consumed, utterly.
The monk stood, wreathed in Primordial Light, and declared themselves Xeroc, the First God.
When a Dragon is laid to rest on the Aerie, its powers are relinquished so they may one day return.
Xeroc renounced their sworn oath and broke the cycle, becoming a traitor without equal.
Word of the ascension spread quickly. Many attempted to follow suit and claim an Auric soul for themselves.
Now you know… Good intentions or no, all Gods are sinners. Each and every one complicit in genocide.
Wherever your journey may lead, whether you are with me or against, may fortune favor you.
For nothing else will.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Requiem");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoonLordTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

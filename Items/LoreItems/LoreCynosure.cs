using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class LoreCynosure : LoreItem
    {
        public override string Lore =>
@"You now stand at the brink of infinity. The power you have amassed is extraordinary.
Valor and deceit, truth and falsehood, loyalty and betrayal… you are beyond these notions.
All, you have rent asunder as they crossed your path. The very land now bends to your will.
Do you not see how the grass parts where you step, how the stars illuminate where you gaze?
Terraria itself kneels to you, whether it be out of fear or respect.
This is the strength the Dragons held. The primordial power they commanded.
Little stands between us now. If you did not seek battle with me, I doubt you would have come so far.
When you are prepared, seek the grave of the Light, at the summit of the Dragon Aerie.
I await your challenge.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Cynosure");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

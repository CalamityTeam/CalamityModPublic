using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    [LegacyName("MysteriousMechanism")]
    public class LabSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Lab Seeking Mechanism");
            Tooltip.SetDefault("A receptacle for technology which pinpoints the power cores of Draedon's Labs");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(4).
                AddIngredient<DubiousPlating>(4).
                AddRecipeGroup("IronBar", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    [LegacyName("MysteriousMechanism")]
    public class LabSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Seeking Mechanism");
            Tooltip.SetDefault("A receptacle for technology which pinpoints the power cores of Draedon's Labs");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(4).
                AddIngredient<DubiousPlating>(4).
                AddIngredient(ItemID.IronBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}

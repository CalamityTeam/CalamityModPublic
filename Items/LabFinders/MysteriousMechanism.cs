using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class MysteriousMechanism : ModItem
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 4).AddIngredient(ModContent.ItemType<DubiousPlating>(), 4).AddIngredient(ItemID.IronBar, 10).AddTile(TileID.Anvils).Register();
        }
    }
}

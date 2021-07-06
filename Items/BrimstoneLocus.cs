using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class BrimstoneLocus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Locus");
            Tooltip.SetDefault("Not only can I enhance your equipment with potent magic, I can also draw out the true strength and ascend\n" +
                "some of your weaponry to entirely new forms\n" +
                "Such items are revealed while this item is in your inventory");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = 0;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CalamitousEssence>());
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 3);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

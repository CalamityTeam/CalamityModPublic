using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class MysteriousMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Mechanism");
            Tooltip.SetDefault("Used as a base for specialized homing technology");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 26;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 4);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 4);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

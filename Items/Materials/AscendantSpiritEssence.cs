using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class AscendantSpiritEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ascendant Spirit Essence");
            Tooltip.SetDefault("A catalyst of the highest caliber formed by fusing powerful souls");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 1.2f * brightness, 0.4f * brightness, 0.8f);
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 54;
            item.maxStack = 999;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = Item.sellPrice(gold: 25);
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

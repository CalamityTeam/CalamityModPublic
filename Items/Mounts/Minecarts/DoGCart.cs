using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts.Minecarts
{
    public class DoGCart : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("The Cart of Gods");

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 36;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;

            item.value = Item.sellPrice(gold: 30);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.UseSound = SoundID.Item68;
            item.noMelee = true;
            item.mountType = ModContent.MountType<DoGCartMount>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 10);
            recipe.AddIngredient(ItemID.Wire, 60);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

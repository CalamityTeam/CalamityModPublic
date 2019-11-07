using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class AuricBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Bar");
            Tooltip.SetDefault("It radiates godly energy");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 15));
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 30;
            item.maxStack = 999;
			item.value = Item.sellPrice(gold: 24);
			item.Calamity().postMoonLordRarity = 15;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
			maxFallSpeed = 0f;
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0.7f * num, 0f);
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BarofLife>());
			recipe.AddIngredient(ModContent.ItemType<AuricOre>(), 20);
			recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 2);
			recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 2);
			recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 2);
			recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
			recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>());
			recipe.AddIngredient(ModContent.ItemType<DarksunFragment>());
			recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>());
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
			recipe.SetResult(this, 2);
			recipe.AddRecipe();
		}
	}
}

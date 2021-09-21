using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class GazeOfCrysthamyr : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaze of Crysthamyr");
            Tooltip.SetDefault("Summons a shadow dragon");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.UseSound = SoundID.NPCHit56;
            item.noMelee = true;
            item.mountType = ModContent.MountType<Crysthamyr>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DD2PetDragon);
            recipe.AddIngredient(ItemID.SoulofNight, 100);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 25);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

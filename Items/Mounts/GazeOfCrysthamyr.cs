using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
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
            item.rare = 10;
            item.value = Item.buyPrice(3, 0, 0, 0);
            item.UseSound = SoundID.NPCHit56;
            item.noMelee = true;
            item.mountType = ModContent.MountType<Crysthamyr>();
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DD2PetDragon);
            recipe.AddIngredient(ItemID.SoulofNight, 100);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 50);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 25);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

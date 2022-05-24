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
            SacrificeTotal = 1;

        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.NPCHit56;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<Crysthamyr>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DD2PetDragon).
                AddIngredient(ItemID.SoulofNight, 100).
                AddIngredient<DarksunFragment>(10).
                AddIngredient<ExodiumClusterOre>(25).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

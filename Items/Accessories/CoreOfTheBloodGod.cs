using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CoreOfTheBloodGod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core of the Blood God");
            Tooltip.SetDefault("8% increased damage and damage reduction\n" +
                "Boosts your max HP by 10%\n" +
                "Healing Potions are 15% more effective\n" +
                "Halves enemy contact damage\n" +
                "When you take contact damage this effect has a 20 second cooldown");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.coreOfTheBloodGod = true;
            modPlayer.fleshTotem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodyWormScarf>()
                .AddIngredient<BloodPact>()
                .AddIngredient<FleshTotem>()
                .AddIngredient<CosmiliteBar>(5)
                .AddIngredient<AscendantSpiritEssence>(4)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
}

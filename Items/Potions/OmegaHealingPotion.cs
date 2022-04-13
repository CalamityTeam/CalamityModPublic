using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class OmegaHealingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Healing Potion");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 32;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.healLife = 300;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.potion = true;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SupremeHealingPotion>().
                AddIngredient<BloodOrb>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

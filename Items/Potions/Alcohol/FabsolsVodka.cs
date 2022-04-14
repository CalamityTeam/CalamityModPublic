using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class FabsolsVodka : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
            DisplayName.SetDefault("Fabsol's Vodka");
            Tooltip.SetDefault("Boosts all damage stats by 8% but lowers defense by 10%\n" +
                               "Increases immune time after being struck\n" +
                               "This magical liquor is highly sought by those with a refined palate");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<FabsolVodkaBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(900f);
            Item.value = Item.buyPrice(0, 2, 60, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Ale).
                AddIngredient(ItemID.PixieDust, 10).
                AddIngredient(ItemID.CrystalShard, 5).
                AddIngredient(ItemID.UnicornHorn).
                AddTile(TileID.Kegs).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.Ale).
                AddIngredient<BloodOrb>(40).
                AddIngredient(ItemID.CrystalShard).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}

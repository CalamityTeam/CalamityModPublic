using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class TitanScalePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Scale Potion");
            Tooltip.SetDefault("Increases knockback, defense by 5 and damage reduction by 5%\n" +
                "Increases defense by 25 and damage reduction by 10% for a few seconds after a true melee strike");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 34;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<TitanScale>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient(ItemID.TitanPotion, 4).
                AddIngredient(ItemID.BeetleHusk).
                AddIngredient<CoralskinFoolfish>().
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe(4).
                AddIngredient(ItemID.BottledWater, 4).
                AddIngredient<BloodOrb>(40).
                AddIngredient(ItemID.BeetleHusk).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}

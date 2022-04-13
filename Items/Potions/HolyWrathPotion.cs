using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class HolyWrathPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Wrath Potion");
            Tooltip.SetDefault("Increases damage by 12% and your attacks inflict holy fire\n" +
                "While this potion's buff is active the Wrath Potion's buff is disabled");
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 36;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Purple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<HolyWrathBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(300f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WrathPotion).
                AddIngredient<UnholyEssence>().
                AddIngredient<GalacticaSingularity>().
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(40).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}

using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class CrumblingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
            DisplayName.SetDefault("Crumbling Potion");
            Tooltip.SetDefault("Increases melee and rogue critical strike chance by 5%\n" +
                "Melee and rogue attacks break enemy armor");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ArmorCrumbling>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.BottledWater, 5).
                AddIngredient<AncientBoneDust>().
                AddIngredient<EssenceofCinder>().
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(20).
                AddIngredient<EssenceofCinder>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}

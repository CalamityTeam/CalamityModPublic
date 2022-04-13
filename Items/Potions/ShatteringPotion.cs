using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class ShatteringPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shattering Potion");
            Tooltip.SetDefault("Increases melee and rogue damage and critical strike chance by 8%\n" +
                "Melee and rogue attacks break enemy armor\n" +
                "While this potion's buff is active the Crumbling Potion's buff is disabled");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ArmorShattering>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrumblingPotion>(2).
                AddIngredient(ItemID.BeetleHusk).
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(30).
                AddIngredient(ItemID.BeetleHusk).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}

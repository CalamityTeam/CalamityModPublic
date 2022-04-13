using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class PenumbraPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Penumbra Potion");
            Tooltip.SetDefault("Rogue stealth generates 15% faster while moving\n"
                + "At night, stealth also generates 15% faster while standing still\n"
                + "During an eclipse both boosts increase to 20%");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<PenumbraBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<SolarVeil>(3).
                AddIngredient(ItemID.LunarTabletFragment).
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(30).
                AddIngredient<SolarVeil>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}

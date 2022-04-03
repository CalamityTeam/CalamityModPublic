using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class TeslaPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Potion");
            Tooltip.SetDefault("Summons an aura of electricity that electrifies and slows enemies\n" +
                "Aura damage is reduced on bosses\n" +
                "Reduces the duration of the Electrified debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<TeslaBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<AerialiteOre>(), 2).AddIngredient(ModContent.ItemType<SeaPrism>(), 5).AddIngredient(ModContent.ItemType<StormlionMandible>()).AddTile(TileID.Bottles).Register();
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<BloodOrb>(), 10).AddTile(TileID.AlchemyTable).Register();
        }
    }
}

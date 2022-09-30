using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class CrumblingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Flask of Crumbling");
            Tooltip.SetDefault("Melee and Whip attacks inflict Armor Crunch on enemies\n" +
                "Rogue attacks inflict Armor Crunch on enemies");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ArmorCrumbling>();
            Item.buffTime = CalamityUtils.SecondsToFrames(1200f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.BottledWater, 5).
                AddIngredient<AncientBoneDust>().
                AddIngredient<EssenceofSunlight>().
                AddTile(TileID.ImbuingStation).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(20).
                AddIngredient<EssenceofSunlight>().
                AddTile(TileID.ImbuingStation).
                Register();
        }
    }
}

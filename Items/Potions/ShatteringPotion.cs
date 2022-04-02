using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<ArmorShattering>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CrumblingPotion>(), 2);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this);
            recipe.AddRecipe();
            // Blood Orb recipes don't get the alchemy table effect
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 30);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

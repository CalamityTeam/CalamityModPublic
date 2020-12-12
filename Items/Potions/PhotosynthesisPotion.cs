using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class PhotosynthesisPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Photosynthesis Potion");
            Tooltip.SetDefault("You regen life quickly while not moving, this effect is five times as strong during daytime\n" +
                "Dropped hearts heal more HP");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 999;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<PhotosynthesisBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BeetleJuice>(), 2);
            recipe.AddIngredient(ItemID.Vine);
            recipe.AddIngredient(ModContent.ItemType<TrapperBulb>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 40);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

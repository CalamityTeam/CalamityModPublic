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
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.consumable = true;
            item.buffType = ModContent.BuffType<PhotosynthesisBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BeetleJuice>());
            recipe.AddIngredient(ModContent.ItemType<TrapperBulb>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this);
            recipe.AddRecipe();
			// Blood orb recipes don't get the alchemy table effect
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

using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class CalamitasBrew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitas' Brew");
            Tooltip.SetDefault("Adds abyssal flames to your melee and rogue projectiles and melee attacks\n" +
                               "Increases your movement speed by 5%");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Lime;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<AbyssalWeapon>();
            item.buffTime = CalamityUtils.SecondsToFrames(900f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 3);
            recipe.AddIngredient(ModContent.ItemType<BrimstoneFish>());
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 3);
            recipe.AddTile(TileID.ImbuingStation);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 20);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>());
            recipe.AddTile(TileID.ImbuingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

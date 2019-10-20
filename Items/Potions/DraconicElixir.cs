using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Buffs.Potions;

namespace CalamityMod.Items.Potions
{
    public class DraconicElixir : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Elixir");
            Tooltip.SetDefault("Greatly increases wing flight time and speed and increases defense by 16\n" +
                "God slayer revival heals you to full HP instead of 150 HP when triggered\n" +
                "Silva invincibility heals you to full HP when triggered\n" +
                "If you trigger the above heals you cannot drink this potion again for 30 seconds");
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
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<DraconicSurgeBuff>();
            item.buffTime = 18000;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Calamity().draconicSurgeCooldown == 0;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>());
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddIngredient(ItemID.Moonglow);
            recipe.AddIngredient(ItemID.Fireblossom);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 50);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

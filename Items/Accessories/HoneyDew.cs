using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HoneyDew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Dew");
            Tooltip.SetDefault("5% increased damage reduction, plus 9 defense and increased life regen while in the Jungle\n" +
            "Poison and Venom immunity\n" +
            "Honey-like life regen with no speed penalty\n" +
            "Most bee/hornet enemies and projectiles do 75% damage to you");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.beeResist = true;

            if (player.ZoneJungle)
            {
                player.lifeRegen += 1;
                player.statDefense += 9;
                player.endurance += 0.05f;
            }

            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[BuffID.Poisoned] = true;

            if (!player.honey && player.lifeRegen < 0)
            {
                player.lifeRegen += 2;
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
            }

            player.lifeRegenTime += 1;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LivingDew>());
            recipe.AddIngredient(ItemID.BottledHoney, 10);
            recipe.AddIngredient(ModContent.ItemType<TrapperBulb>(), 2);
            recipe.AddIngredient(ItemID.ButterflyDust);
            recipe.AddIngredient(ModContent.ItemType<BeetleJuice>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

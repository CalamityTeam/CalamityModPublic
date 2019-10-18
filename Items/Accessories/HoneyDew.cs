using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class HoneyDew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Dew");
            Tooltip.SetDefault("5% increased damage reduction, +5 defense, and increased life regen while in the Jungle\n" +
            "Poison and Venom immunity\n" +
            "Honey-like life regen with no speed penalty\n" +
            "Most bee/hornet enemies and projectiles do 75% damage to you");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.beeResist = true;
            if (player.ZoneJungle)
            {
                player.lifeRegen += 1;
                player.statDefense += 5;
                player.endurance += 0.05f;
            }
            player.buffImmune[70] = true;
            player.buffImmune[20] = true;
            if (!player.honey && player.lifeRegen < 0)
            {
                player.lifeRegen += 2;
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
            }
            player.lifeRegenTime += 1;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "LivingDew");
            recipe.AddIngredient(ItemID.BottledHoney, 10);
            recipe.AddIngredient(ItemID.BeeWax, 10);
            recipe.AddIngredient(ItemID.Bezoar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

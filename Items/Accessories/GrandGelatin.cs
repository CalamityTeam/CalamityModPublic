using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class GrandGelatin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Gelatin");
            Tooltip.SetDefault("10% increased movement speed\n" +
                "200% increased jump speed\n" +
                "+20 max life and mana\n" +
                "Standing still boosts life and mana regen");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 6;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += player.autoJump ? 0.5f : 2.0f;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 20;
            if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
            {
                player.lifeRegen += 2;
                player.manaRegenBonus += 2;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ManaJelly");
            recipe.AddIngredient(null, "LifeJelly");
            recipe.AddIngredient(null, "VitalJelly");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

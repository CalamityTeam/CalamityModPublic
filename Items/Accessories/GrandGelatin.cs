using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GrandGelatin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Gelatin");
            Tooltip.SetDefault("10% increased movement and jump speed\n" +
                "+20 max life and mana\n" +
                "Standing still boosts life and mana regen");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += 0.5f;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 20;
            if (Math.Abs(player.velocity.X) < 0.05f && Math.Abs(player.velocity.Y) < 0.05f && player.itemAnimation == 0)
            {
                player.lifeRegen += 4;
                player.manaRegenBonus += 4;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ManaJelly>());
            recipe.AddIngredient(ModContent.ItemType<LifeJelly>());
            recipe.AddIngredient(ModContent.ItemType<VitalJelly>());
            recipe.AddIngredient(ItemID.SoulofLight, 2);
            recipe.AddIngredient(ItemID.SoulofNight, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

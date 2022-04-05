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
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
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
            CreateRecipe()
                .AddIngredient<ManaJelly>()
                .AddIngredient<LifeJelly>()
                .AddIngredient<VitalJelly>()
                .AddIngredient(ItemID.SoulofLight, 2)
                .AddIngredient(ItemID.SoulofNight, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

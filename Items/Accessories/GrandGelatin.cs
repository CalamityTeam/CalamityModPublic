using System;
using System.Collections.Generic;
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
            Tooltip.SetDefault("10% increased movement speed\n" +
                "40% increased jump speed\n" +
                "+20 max life and mana\n" +
                "Standing still boosts life and mana regen");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            bool autoJump = Main.player[Main.myPlayer].autoJump;
			string jumpAmt = autoJump ? "10" : "40";
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = jumpAmt + "% increased jump speed";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += player.autoJump ? 0.5f : 2f;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 20;
            if (Math.Abs(player.velocity.X) < 0.05f && Math.Abs(player.velocity.Y) < 0.05f && player.itemAnimation == 0)
            {
                player.lifeRegen += 2;
                player.manaRegenBonus += 2;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ManaJelly>());
            recipe.AddIngredient(ModContent.ItemType<LifeJelly>());
            recipe.AddIngredient(ModContent.ItemType<VitalJelly>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

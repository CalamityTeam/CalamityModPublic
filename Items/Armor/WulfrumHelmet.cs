using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Helmet");
            Tooltip.SetDefault("3% increased summon damage\n" +
                               "+1 max minion");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 20000;
            item.rare = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("WulfrumArmor") && legs.type == mod.ItemType("WulfrumLeggings");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 defense\n" +
                "+5 defense when below 50% life";
            player.statDefense += 3; //8
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
            {
                player.statDefense += 5; //13
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
            player.maxMinions++;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "WulfrumShard", 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
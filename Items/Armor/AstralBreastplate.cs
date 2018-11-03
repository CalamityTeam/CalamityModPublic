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
    [AutoloadEquip(EquipType.Body)]
    public class AstralBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Breastplate");
            Tooltip.SetDefault("+20 max mana and life\n" +
                               "+2 max minions\n" +
                               "Creature detection");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 470000;
            item.rare = 7;
            item.defense = 23;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.statManaMax2 += 20;
            player.maxMinions += 2;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AstralBar", 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
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
    public class TarragonHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Helmet");
            Tooltip.SetDefault("Immune to lava, cursed inferno, fire, cursed, and chilled debuffs\n" +
                "Can move freely through liquids\n" +
                "10% increased throwing damage and critical strike chance\n" +
                "5% increased damage reduction");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1550000;
            item.defense = 15; //98
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("TarragonBreastplate") && legs.type == mod.ItemType("TarragonLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            player.setBonus = "Reduces enemy spawn rates\n" +
                "Increased heart pickup range\n" +
                "Enemies have a chance to drop extra hearts on death\n" +
                "After every 25 throwing critical hits you will gain 5 seconds of damage immunity\n" +
                "This effect can only occur once every 30 seconds\n" +
                "While under the effects of a debuff you gain 10% increased throwing damage";
        }

        public override void UpdateEquip(Player player)
        {
            player.thrownDamage += 0.1f;
            player.thrownCrit += 10;
            player.endurance += 0.05f;
            player.lavaImmune = true;
            player.ignoreWater = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Chilled] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar", 7);
            recipe.AddIngredient(null, "DivineGeode", 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
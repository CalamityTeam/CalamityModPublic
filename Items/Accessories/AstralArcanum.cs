using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class AstralArcanum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Arcanum");
            Tooltip.SetDefault("Taking damage drops astral stars from the sky\n" +
                               "Provides immunity to the god slayer inferno debuff\n" +
                               "You have a 10% chance to reflect projectiles when they hit you\n" +
                               "If this effect triggers you get healed for the projectile's damage\n" +
                               "Boosts life regen even while under the effects of a damaging debuff\n" +
                               "While under the effects of a damaging debuff you will gain 40 defense\n" +
                               "Press O to toggle teleportation UI");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 1000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.astralArcanum = true;
            modPlayer.aBulwark = true;
            modPlayer.projRef = true;
            player.buffImmune[mod.BuffType("GodSlayerInferno")] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CelestialJewel");
            recipe.AddIngredient(null, "AstralBulwark");
            recipe.AddIngredient(null, "ArcanumoftheVoid");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
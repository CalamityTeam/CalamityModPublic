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
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardianAegis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asgardian Aegis");
            Tooltip.SetDefault("Grants immunity to fire blocks and knockback\n" +
                "Immune to most debuffs\n" +
                "+40 max life\n" +
                "Grants a supreme holy flame dash\n" +
                "Can be used to ram enemies\n" +
                "Press N to activate buffs to all damage, crit chance, and defense\n" +
                "Activating this buff will reduce your movement speed and increase enemy aggro\n" +
                "10% damage reduction while submerged in liquid\n" +
                "Increased defense by 10 when below 25% life\n" +
                "Toggle visibility of this accessory to enable/disable the dash");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.height = 54;
            item.value = Item.buyPrice(0, 90, 0, 0); //30 gold reforge
            item.defense = 10;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (!hideVisual) { modPlayer.dashMod = 4; }
            modPlayer.elysianAegis = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.statLifeMax2 += 40;
            player.buffImmune[46] = true;
            player.buffImmune[44] = true;
            player.buffImmune[33] = true;
            player.buffImmune[36] = true;
            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
            player.buffImmune[32] = true;
            player.buffImmune[31] = true;
            player.buffImmune[35] = true;
            player.buffImmune[23] = true;
            player.buffImmune[22] = true;
            player.buffImmune[mod.BuffType("BrimstoneFlames")] = true;
            player.buffImmune[mod.BuffType("HolyLight")] = true;
            player.buffImmune[mod.BuffType("GlacialState")] = true;
            if (player.statLife <= (int)((double)player.statLifeMax2 * 0.25)) { player.statDefense += 10; }
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)) { player.endurance += 0.1f; }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AsgardsValor");
            recipe.AddIngredient(null, "ElysianAegis");
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
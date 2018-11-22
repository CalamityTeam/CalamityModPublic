using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Helm");
            Tooltip.SetDefault("13% increased melee damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 9000000;
            item.defense = 52; //110
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(108, 45, 199);
                }
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("SilvaArmor") && legs.type == mod.ItemType("SilvaLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.silvaSet = true;
            modPlayer.silvaMelee = true;
            player.setBonus = "You are immune to almost all debuffs\n" +
                "Reduces all damage taken by 5%, this is calculated separately from damage reduction\n" +
                "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 1 HP you will not die from any further damage for 10 seconds\n" +
                "If you get reduced to 1 HP again while this effect is active you will lose 100 max life\n" +
                "This effect only triggers once per life\n" +
                "Your max life will return to normal if you die\n" +
                "True melee strikes have a 25% chance to do five times damage\n" +
                "After the silva invincibility is over you will take halved contact damage\n" +
                "Melee projectiles have a 25% chance to stun enemies for a very brief moment";
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.13f;
            player.meleeCrit += 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "EffulgentFeather", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "NightmareFuel", 14);
            recipe.AddIngredient(null, "EndothermicEnergy", 14);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
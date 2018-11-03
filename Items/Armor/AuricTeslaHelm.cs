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
    public class AuricTeslaHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Royal Helm");
            Tooltip.SetDefault("20% increased melee damage and critical strike chance\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000000;
            item.defense = 54; //132
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AuricTeslaBodyArmor") && legs.type == mod.ItemType("AuricTeslaCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Melee Tarragon, Bloodflare, God Slayer, and Silva armor effects\n" +
                "Reduces all damage taken by 10%, this is calculated separately from damage reduction\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 10%\n" +
                "Your melee damage is multiplied based on how high your HP is; at full HP this effect is at max";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.tarraSet = true;
            modPlayer.tarraMelee = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMelee = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerDamage = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaMelee = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.lavaImmune = true;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            player.aggro += 1200;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.auricBoost = true;
            player.meleeDamage += 0.2f;
            player.meleeCrit += 20;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaHelm");
            recipe.AddIngredient(null, "GodSlayerHelm");
            recipe.AddIngredient(null, "BloodflareMask");
            recipe.AddIngredient(null, "TarragonHelm");
            recipe.AddIngredient(null, "AuricOre", 60);
            recipe.AddIngredient(null, "EndothermicEnergy", 10);
            recipe.AddIngredient(null, "NightmareFuel", 10);
            recipe.AddIngredient(null, "Phantoplasm", 8);
            recipe.AddIngredient(null, "DarksunFragment", 6);
            recipe.AddIngredient(null, "BarofLife", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 5);
            recipe.AddIngredient(null, "CoreofCalamity", 2);
            recipe.AddIngredient(null, "GalacticaSingularity");
            recipe.AddIngredient(null, "PsychoticAmulet");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
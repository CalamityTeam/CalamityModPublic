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
    public class AuricTeslaPlumedHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Plumed Helm");
            Tooltip.SetDefault("20% increased throwing damage and critical strike chance\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000000;
            item.defense = 34; //132
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
            player.setBonus = "Throwing Tarragon, Bloodflare, God Slayer, and Silva armor effects\n" +
                "Reduces all damage taken by 10%, this is calculated separately from damage reduction\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 10%\n" +
                "Throwing weapon critical strikes will do 3.5 times damage while you are above 90% HP";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareThrowing = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaThrowing = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.lavaImmune = true;
            player.ignoreWater = true;
            player.crimsonRegen = true;
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
            player.thrownDamage += 0.2f;
            player.thrownCrit += 20;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaMask");
            recipe.AddIngredient(null, "GodSlayerMask");
            recipe.AddIngredient(null, "BloodflareHelm");
            recipe.AddIngredient(null, "TarragonHelmet");
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
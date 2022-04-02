using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Nincity
    public class AngelicAlliance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angelic Alliance");
            Tooltip.SetDefault("Call upon the force of heaven to empower your attacks and minions\n" +
            "Courage, Enlightenment, Bliss. United in Judgement\n" +
            "+2 max minions, 15% increased summon damage, and 8% increased damage to all other classes\n" +
            "Life regeneration is boosted while jumping\n" +
            "This line is modified in the code below. If you can read this, someone probably did something wrong (It was Ben)\n" +
            "While under the effects of Divine Bless, for every minion you have, an archangel shall be summoned to aid you in combat\n" +
            "Each spawned angel will instantly heal you for two health\n" +
            "All minion attacks inflict Banishing Fire and you are granted a flat health boost of four health per second\n" +
            "This effect has a cooldown of 1 minute");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 92;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.accessory = true;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AngelicAllianceHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
                {
                    line2.text = "Press " + hotkey + " to grace yourself in divinity for 15 seconds";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.angelicAlliance = true;
            player.allDamage += 0.08f;
            player.minionDamage += 0.07f; //7% + 8% = 15%
            player.maxMinions += 2;
            if (player.controlJump)
                player.lifeRegen += 4;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyHallowedHelmet");
            //recipe.AddRecipeGroup("AnyHallowedPlatemail");
            //recipe.AddRecipeGroup("AnyHallowedGreaves");
            recipe.AddIngredient(ItemID.HallowedPlateMail);
            recipe.AddIngredient(ItemID.HallowedGreaves);
            recipe.AddIngredient(ItemID.PaladinsShield);
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

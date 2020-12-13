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
			"Further Empowerment to Holy Forces\n" +
			"Courage, Enlightenment, Bliss. United in Judgement\n" +
			"10% increased damage\n" +
			"While holding the Profaned Guardians Lore item, all attacks have a chance to inflict Banishing Fire\n" +
			"While holding the Providence Lore item, classless projectiles deal increased damage and the Holy Flames debuff deals more damage\n" +
			"While the Profaned Soul Crystal is equipped, additional guardians will be summoned for a total of nine\n" + //This is going to be broken as fuck because the angels aren't already, y'know?
			"This line is modified below. If you can read this, someone probably did something wrong (It was Ben)\n" +
			"While under the effects of Divine Bless, for every minion you have, an archangel shall be summoned to aid you in combat\n" +
			"Divine Bless also boosts your life regeneration and grants an additional +5% increased damage\n" +
			"This line is also modified below, kinda sus");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 96;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.accessory = true;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Developer;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AngelicAllianceHotKey.TooltipHotkeyString();
			bool crystal = Main.player[Main.myPlayer].Calamity().profanedCrystal;
			int time = crystal ? 60 : 30;
			string time2 = crystal ? "1.5" : "2";
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip7")
                {
                    line2.text = "Press " + hotkey + " to grace yourself in divinity for " + time + " seconds";
                }
                if (line2.mod == "Terraria" && line2.Name == "Tooltip10")
                {
                    line2.text = "This effect has a cooldown of " + time2 + " minutes";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.angelicAlliance = true;
			player.allDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            PSCRecipe recipe = new PSCRecipe(mod);
            recipe.AddRecipeGroup("AnyHallowedHelmet");
            //recipe.AddRecipeGroup("AnyHallowedPlatemail");
            //recipe.AddRecipeGroup("AnyHallowedGreaves");
            recipe.AddIngredient(ItemID.HallowedPlateMail);
            recipe.AddIngredient(ItemID.HallowedGreaves);
            recipe.AddIngredient(ModContent.ItemType<ElysianAegis>());
            recipe.AddIngredient(ItemID.Excalibur);
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

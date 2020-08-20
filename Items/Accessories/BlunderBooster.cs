using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class BlunderBooster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blunder Booster");
            Tooltip.SetDefault("12% increased rogue damage and 15% increased rogue projectile velocity\n" +
                "Summons a red lightning aura to surround the player and electrify nearby enemies\n" +
                "TOOLTIP LINE HERE" + 
                "This effect has a 3 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = 10;
			item.Calamity().postMoonLordRarity = 12;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.Calamity().hasJetpack = true;
            player.Calamity().throwingDamage += 0.12f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().blunderBooster = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.PlaguePackHotKey.TooltipHotkeyString();
            foreach (TooltipLine line in list)
            {
                if (line.mod == "Terraria" && line.Name == "Tooltip2")
                {
                    line.text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of lightning bolts";
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PlaguedFuelPack>());
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

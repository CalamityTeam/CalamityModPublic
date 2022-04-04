using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace CalamityMod.Items.Accessories
{
    public class BlunderBooster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blunder Booster");
            Tooltip.SetDefault("12% increased rogue damage and 15% increased rogue projectile velocity\n" +
                "Stealth generates 10% faster\n" +
                "Summons a red lightning aura to surround the player and electrify nearby enemies\n" +
                "TOOLTIP LINE HERE" +
                "This effect has a 1 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        //Todo - Check if its trying to replace the other rogue jetpack. If its the case, return true.
        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hasJetpack = true;
            player.Calamity().throwingDamage += 0.12f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().blunderBooster = true;
            player.Calamity().stealthGenStandstill += 0.1f;
            player.Calamity().stealthGenMoving += 0.1f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.PlaguePackHotKey.GetAssignedKeys().Aggregate((x, y) => x + ", " + y);
            foreach (TooltipLine line in list)
            {
                if (line.Mod == "Terraria" && line.Name == "Tooltip3")
                {
                    line.text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of lightning bolts";
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlaguedFuelPack>()).AddIngredient(ModContent.ItemType<EffulgentFeather>(), 8).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}

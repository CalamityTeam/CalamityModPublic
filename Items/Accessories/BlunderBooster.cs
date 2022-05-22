using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class BlunderBooster : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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

        // TODO -- Check if its trying to replace the other rogue jetpack. If its the case, return true.
        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hasJetpack = true;
            player.Calamity().throwingDamage += 0.12f;
            player.Calamity().rogueVelocity += 0.15f;
            player.Calamity().blunderBooster = true;
            player.Calamity().stealthGenStandstill += 0.1f;
            player.Calamity().stealthGenMoving += 0.1f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.PlaguePackHotKey.TooltipHotkeyString();
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip3");

            if (line != null)
                line.Text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of lightning bolts";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlaguedFuelPack>().
                AddIngredient<EffulgentFeather>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

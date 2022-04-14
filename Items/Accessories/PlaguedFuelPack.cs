using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class PlaguedFuelPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Plagued Fuel Pack");
            Tooltip.SetDefault("5% increased rogue damage and 15% increased rogue projectile velocity\n" +
                "Stealth generates 10% faster\n" +
                "TOOLTIP LINE HERE" +
                "This effect has a 1 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        //Todo : Check if the slot contains the other rogue jetpack, in which case, let the player swap accs
        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hasJetpack = true;
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().plaguedFuelPack = true;
            player.Calamity().stealthGenStandstill += 0.1f;
            player.Calamity().stealthGenMoving += 0.1f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.PlaguePackHotKey.TooltipHotkeyString();
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null)
                line.Text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of plagued clouds";
        }
    }
}

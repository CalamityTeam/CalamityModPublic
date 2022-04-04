using CalamityMod.Items.Placeables;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Potions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace CalamityMod.Items.Accessories
{
    public class CelestialJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Jewel");
            Tooltip.SetDefault("Boosts life regen even while under the effects of a damaging debuff\n" +
                "While under the effects of a damaging debuff you will gain 11 defense\n" +
                "TOOLTIP LINE HERE");
        }

        public override void SetDefaults()
        {
            Item.defense = 8;
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.AstralTeleportHotKey.GetAssignedKeys().Aggregate((x, y) => x + ", " + y);
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "Tooltip2")
                {
                    line2.text = "Press " + hotkey + " to teleport to a random location while no bosses are alive";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.celestialJewel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CrownJewel>()).AddIngredient(ItemID.TeleportationPotion, 3).AddIngredient(ModContent.ItemType<AstralJelly>(), 15).AddIngredient(ModContent.ItemType<SeaPrism>(), 15).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}

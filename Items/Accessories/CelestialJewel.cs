using CalamityMod.Items.Placeables;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Potions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.defense = 8;
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AstralTeleportHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip2")
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CrownJewel>());
            recipe.AddIngredient(ItemID.TeleportationPotion, 3);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 15);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

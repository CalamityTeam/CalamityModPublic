using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AstralArcanum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Arcanum");
            Tooltip.SetDefault("Provides immunity to the astral infection debuff\n" +
                "Boosts life regen even while under the effects of a damaging debuff\n" +
                "While under the effects of a damaging debuff you will gain 15 defense\n" +
                "TOOLTIP LINE HERE");
        }

        public override void SetDefaults()
        {
            item.defense = 12;
            item.width = 26;
            item.height = 26;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.rare = ItemRarityID.Purple;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AstralArcanumUIHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip3")
                {
                    line2.text = "Press " + hotkey + " to toggle teleportation UI while no bosses are alive";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.astralArcanum = true;
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CelestialJewel>());
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 3);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

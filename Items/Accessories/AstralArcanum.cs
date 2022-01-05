using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
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
            Tooltip.SetDefault("Taking damage drops astral stars from the sky\n" +
                "Provides immunity to the astral infection debuff\n" +
                "You reflect projectiles when they hit you\n" +
                "Reflected projectiles deal no damage to you\n" +
                "This reflect has a 90 second cooldown which is shared with all other dodges and reflects\n" +
                "Boosts life regen even while under the effects of a damaging debuff\n" +
                "While under the effects of a damaging debuff you will gain 15 defense\n" +
                "TOOLTIP LINE HERE");
        }

        public override void SetDefaults()
        {
			item.defense = 12;
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.accessory = true;
            item.expert = true;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AstralArcanumUIHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip7")
                {
                    line2.text = "Press " + hotkey + " to toggle teleportation UI while no bosses are alive";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.astralArcanum = true;
            modPlayer.aBulwark = true;
            modPlayer.projRef = true;
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CelestialJewel>());
            recipe.AddIngredient(ModContent.ItemType<AstralBulwark>());
            recipe.AddIngredient(ModContent.ItemType<ArcanumoftheVoid>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

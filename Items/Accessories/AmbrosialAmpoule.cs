using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmbrosialAmpoule : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int MinRegenBoost => 2;
        public static int MaxRegenBoost => 6;
        public static float RegenTimeBoost => 1;
        public static float NaturalRegenPower => 1.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinRegenBoost.ToRegenPerSecond(), MaxRegenBoost.ToRegenPerSecond(), (NaturalRegenPower - 1f).ToPercent(), RegenTimeBoost.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aAmpoule = true;
            if (!modPlayer.purity && !hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1.2f, 1.2f, 0.72f));
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            var player = Main.LocalPlayer;
            if (player != null)
            {
                list.FindAndReplace("[REGEN]", ((int)MathF.Round(MathHelper.Lerp(MaxRegenBoost, MinRegenBoost, (player.statLife / (float)player.statLifeMax2)))).ToRegenPerSecond());
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LivingDew>().
                AddIngredient<RadiantOoze>().
                AddIngredient<LifeAlloy>(3).
                AddIngredient(ItemID.FragmentSolar, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

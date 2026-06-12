using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("AstralArcanum", "Purity")]
    public class Radiance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int MinRegenBoost => 2;
        public static int MaxRegenBoost => 8;
        public static int ReducedDoTAmount => 20;
        public static int PostDebuffRegenTimeBoost => CalamityUtils.SecondsToFrames(10);
        public static float RegenTimeBoost => 1;
        public static float NaturalRegenPower => 1.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinRegenBoost.ToRegenPerSecond(),MaxRegenBoost.ToRegenPerSecond(),ReducedDoTAmount.ToRegenPerSecond(), (NaturalRegenPower - 1f).ToPercent(),RegenTimeBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 44;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.purity = true;
            if (!hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1.32f, 1.32f, 1.82f));
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
                AddIngredient<AmbrosialAmpoule>().
                AddIngredient<InfectedJewel>().
                AddIngredient<AuricBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}

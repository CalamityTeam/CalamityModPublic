using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    [LegacyName("SunkenStew")]
    public class HadalStew : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public static int BuffType = BuffID.WellFed2;
        public static int BuffDuration = 60 * 3600;
        public static int SicknessDuration = 50 * 60;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffDuration / 3600);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.potion = true;
            Item.healLife = 120;
            Item.healMana = 150;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string duration = Main.LocalPlayer.pStone ? (SicknessDuration / 60 * 0.75f).ToString("N1") : (SicknessDuration / 60).ToString();
            list.FindAndReplace("[S]", duration);
        }

        public override bool CanUseItem(Player player)
        {
            return player.potionDelay <= 0 && player.Calamity().potionTimer <= 0;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType, BuffDuration);
            // fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
            player.Calamity().potionTimer = 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AbyssGravel>(10).
                AddIngredient<CoastalDemonfish>().
                AddIngredient(ItemID.Honeyfin).
                AddIngredient(ItemID.Bowl, 2).
                AddTile(TileID.CookingPots).
                Register();

            CreateRecipe(2).
                AddIngredient<Voidstone>(10).
                AddIngredient<CoastalDemonfish>().
                AddIngredient(ItemID.Honeyfin).
                AddIngredient(ItemID.Bowl, 2).
                AddTile(TileID.CookingPots).
                Register();
        }
    }
}

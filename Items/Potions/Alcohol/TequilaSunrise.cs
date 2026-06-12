using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class TequilaSunrise : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float DoTMultiplier = 1.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DoTMultiplier-1).ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.TequilaSunrise.DripEffect").WithFormatArgs((DoTMultiplier-1).ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.TequilaSunrise;

        public Action<Player, float> IVDripAlcoholEffect => ApplyTequilaSunriseEffect;

        private static void ApplyTequilaSunriseEffect(Player player, float intensity)
        {
            // See CalamityPlayerLifeRegen
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(220, 75, 59),
                new Color(229, 147, 47),
                new Color(255, 190, 76)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 28, ModContent.BuffType<TequilaSunriseBuff>(), CalamityUtils.MinutesToFrames(6), true);
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.LightShard).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 9 * player.direction;
            player.itemLocation.Y -= 10;
        }
    }
}

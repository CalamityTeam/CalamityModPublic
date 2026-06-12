using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Everclear : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float DamageBoost = 0.25f;
        public static int RegenLoss = 10;
        public static float DefenseLossPercent = 0.30f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), RegenLoss.ToRegenPerSecond(), DefenseLossPercent.ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Everclear.DripEffect").WithFormatArgs(DamageBoost.ToPercent(), RegenLoss.ToRegenPerSecond(), DefenseLossPercent.ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.Everclear;

        public Action<Player, float> IVDripAlcoholEffect => ApplyEverclearEffect;

        private static void ApplyEverclearEffect(Player player, float intensity)
        {
            // See CalamityPlayerLifeRegen and CalamityPlayerMiscEffects
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[2] {
                new Color(153, 168, 162, 180),
                new Color(198, 205, 207, 180)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(16, 52, ModContent.BuffType<EverclearBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.Bottle, 20).
                AddIngredient(ItemID.BlackLens).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 3 * player.direction;
            player.itemLocation.Y -= 7;
        }
    }
}

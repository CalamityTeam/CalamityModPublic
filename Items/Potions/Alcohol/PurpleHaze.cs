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
    [LegacyName("FabsolsVodka")]
    public class PurpleHaze : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float DamageBoost = 0.25f;
        public static float StealthDamageLoss = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(),StealthDamageLoss.ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.PurpleHaze.DripEffect").WithFormatArgs(DamageBoost.ToPercent(), StealthDamageLoss.ToPercent());

        public AlcoholType AlcoholVariant => AlcoholType.PurpleHaze;

        public Action<Player, float> IVDripAlcoholEffect => ApplyPurpleHazeEffect;

        private static void ApplyPurpleHazeEffect(Player player, float intensity)
        {
            // See CalamityPlayer and CalamityPlayerMiscEffects
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(239, 123, 202),
                new Color(187, 56, 158),
                new Color(165, 47, 255)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 48, ModContent.BuffType<PurpleHazeBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Plum).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 4 * player.direction;
            player.itemLocation.Y -= 8;
        }
    }
}

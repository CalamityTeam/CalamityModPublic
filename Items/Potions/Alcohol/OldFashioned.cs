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
    public class OldFashioned : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static readonly float DamageBoostMultiplier = 1.25f;
        public static readonly float DamageReductionMultiplier = 0.75f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DamageBoostMultiplier).ToString("N2"), (DamageReductionMultiplier).ToString("N2"));
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.OldFashioned.DripEffect").WithFormatArgs(DamageBoostMultiplier.ToString("N2"), DamageReductionMultiplier.ToString("N2"));
        public AlcoholType AlcoholVariant => AlcoholType.OldFashioned;

        public Action<Player, float> IVDripAlcoholEffect => ApplyOldFashionedEffect;

        private static void ApplyOldFashionedEffect(Player player, float intensity)
        {
            // See CalamityGlobalProjectile
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(255, 118, 3),
                new Color(255, 200, 82),
                new Color(255, 228, 122)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(38, 44, ModContent.BuffType<OldFashionedBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Grapefruit).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 3 * player.direction;
            player.itemLocation.Y -= 12;
        }
    }
}

using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class GrapeBeer : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float CloseRangeDistance = 80;
        public static float LongRangeDistance = 1200;
        public static float CloseRangeDamage = 0.75f;
        public static float LongRangeDamage = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LongRangeDamage, LongRangeDistance / 16f); 
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.GrapeBeer.DripEffect").WithFormatArgs(LongRangeDamage, LongRangeDistance / 16f);
        public AlcoholType AlcoholVariant => AlcoholType.GrapeBeer;

        public Action<Player, float> IVDripAlcoholEffect => ApplyGrapeBeerEffect;

        private static void ApplyGrapeBeerEffect(Player player, float intensity)
        {
            // See CalamityGlobalProjectile
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(36, 2, 41),
                new Color(56, 0, 64),
                new Color(82, 10, 92)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 48, ModContent.BuffType<GrapeBeerBuff>(), CalamityUtils.MinutesToFrames(6), true);
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Grapes).
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

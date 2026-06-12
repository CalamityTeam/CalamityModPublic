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
    public class CaribbeanRum : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float MoveSpeedBoost = 0.25f;
        public static float GravityMultiplier = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBoost.ToPercent(), (1-GravityMultiplier).ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.CaribbeanRum.DripEffect").WithFormatArgs(MoveSpeedBoost.ToPercent(), (1 - GravityMultiplier).ToPercent());

        public AlcoholType AlcoholVariant => AlcoholType.CaribbeanRum;

        public Action<Player, float> IVDripAlcoholEffect => ApplyCaribbeanRumEffect;

        private static void ApplyCaribbeanRumEffect(Player player, float intensity)
        {
            // See CalamityPlayerMiscEffects
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(105, 29, 14),
                new Color(128, 39, 22),
                new Color(138, 28, 7)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(28, 52, ModContent.BuffType<CaribbeanRumBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.Bottle, 20).
                AddIngredient(ItemID.IceFeather).
                AddTile(TileID.Kegs).
                Register();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 5 * player.direction;
            player.itemLocation.Y -= 9;
        }
    }
}

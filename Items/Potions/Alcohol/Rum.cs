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
    public class Rum : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float NonMinionBoost = 1.15f;
        public static float MinionBoost = 0.85f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((NonMinionBoost-1).ToPercent(), (1- MinionBoost).ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Rum.DripEffect").WithFormatArgs((NonMinionBoost - 1).ToPercent(), (1 - MinionBoost).ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.Rum;

        public Action<Player, float> IVDripAlcoholEffect => ApplyRumEffect;

        private static void ApplyRumEffect(Player player, float intensity)
        {
            // See CalamityGlobalProjectile
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(237, 165, 9),
                new Color(247, 219, 54),
                new Color(255, 195, 31)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(30, 44, ModContent.BuffType<RumBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.SpiderFang, 3).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 3 * player.direction;
            player.itemLocation.Y -= 9;
        }
    }
}

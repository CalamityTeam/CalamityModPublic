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
    public class EvergreenGin : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.EvergreenGin.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.EvergreenGin;

        public Action<Player, float> IVDripAlcoholEffect => ApplyEvergreenGinEffect;
        private static void ApplyEvergreenGinEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.SicknessDebuffMultiplier += EvergreenGin.DebuffBoost;
            cplayer.WaterDebuffMultiplier += EvergreenGin.DebuffBoost;
            cplayer.ElectricDebuffMultiplier -= EvergreenGin.DebuffLoss;
            cplayer.HeatDebuffMultiplier -= EvergreenGin.DebuffLoss;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(201, 248, 248),
                new Color(201, 248, 221),
                new Color(177, 240, 184)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(34, 56, ModContent.BuffType<EvergreenGinBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Vine, 3).
                AddIngredient<StarblightSoot>(10).
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

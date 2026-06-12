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
    public class Vodka : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float DebuffBoost = 0.25f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Vodka.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.Vodka;

        public Action<Player, float> IVDripAlcoholEffect => ApplyVodkaEffect;

        private static void ApplyVodkaEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.TypelessDebuffMultiplier += Vodka.DebuffBoost;
            cplayer.HeatDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.ColdDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.SicknessDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.WaterDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.ElectricDebuffMultiplier -= Vodka.DebuffLoss;
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[2] {
                new Color(219, 219, 208, 180),
                new Color(181, 181, 176, 180)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(14, 36, ModContent.BuffType<VodkaBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient<PurifiedGel>(10).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 2 * player.direction;
            player.itemLocation.Y -= 8;
        }
    }
}

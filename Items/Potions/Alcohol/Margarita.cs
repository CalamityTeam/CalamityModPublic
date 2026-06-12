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
    public class Margarita : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static int BuffType = ModContent.BuffType<MargaritaBuff>();
        public static float DebuffLoss = 0.5f;
        public static int MinuteDuration = 3; 
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DebuffLoss.ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Margarita.DripEffect").WithFormatArgs(DebuffLoss.ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.Margarita;

        public Action<Player, float> IVDripAlcoholEffect => ApplyMargaritaEffect;

        private static void ApplyMargaritaEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.HeatDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.SicknessDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.ColdDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.WaterDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.ElectricDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.TypelessDebuffMultiplier -= Margarita.DebuffLoss;
            // Also see CalamityPlayerMiscEffects
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(219, 227, 191),
                new Color(186, 189, 147),
                new Color(142, 161, 125)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(42, 52, ModContent.BuffType<MargaritaBuff>(), CalamityUtils.MinutesToFrames(6), true);
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.DarkShard).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 9 * player.direction;
            player.itemLocation.Y -= 14;
        }
    }
}

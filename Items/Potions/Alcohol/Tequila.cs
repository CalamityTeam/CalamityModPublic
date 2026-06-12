using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Tequila : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";


        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Tequila.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public AlcoholType AlcoholVariant => AlcoholType.Tequila;

        public Action<Player, float> IVDripAlcoholEffect => ApplyTequilaEffect;

        private static void ApplyTequilaEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.ElectricDebuffMultiplier += Tequila.DebuffBoost;
            cplayer.ColdDebuffMultiplier -= Tequila.DebuffLoss;
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(163, 110, 10),
                new Color(176, 135, 0),
                new Color(194, 132, 25)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(32, 44, ModContent.BuffType<TequilaBuff>(), CalamityUtils.MinutesToFrames(6), true);
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient<StormlionMandible>(3).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 4 * player.direction;
            player.itemLocation.Y -= 9;
        }
    }
}

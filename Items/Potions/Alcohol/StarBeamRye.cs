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
    public class StarBeamRye : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float MagicDmgMult = 0.9f;
        public static int ManaRegenBoost = 30;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicDmgMult);
        public AlcoholType AlcoholVariant => AlcoholType.StarBeamRye;
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.StarBeamRye.DripEffect").WithFormatArgs(MagicDmgMult);

        public Action<Player, float> IVDripAlcoholEffect => ApplyStarBeamRyeEffect;

        private static void ApplyStarBeamRyeEffect(Player player, float intensity)
        {
            // See CalamityPlayerMiscEffects
        }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(89, 18, 10),
                new Color(102, 39, 14),
                new Color(128, 31, 9)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 54, ModContent.BuffType<StarBeamRyeBuff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Starfruit).
                AddTile(TileID.Kegs).
                Register();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 4 * player.direction;
            player.itemLocation.Y -= 7;
        }
    }
}

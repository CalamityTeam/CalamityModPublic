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
    public class BloodyMary : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public static float PierceDamageMultiplier = 0.66f;
        public static float SpawnRateGateMultiplier = 0.142f;
        public static float IVDripAdditionalSpawnRateGateMultiplier = 0.7f;
        public static float SpawnLimitMultiplier = 5f;
        public static float IVDripAdditionalSpawnLimitMultiplier = 1.5f;        
        public override LocalizedText Tooltip => base.Tooltip;
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.BloodyMary.DripEffect");
        public AlcoholType AlcoholVariant => AlcoholType.BloodyMary;

        public Action<Player, float> IVDripAlcoholEffect => ApplyBloodyMaryEffect;

        private static void ApplyBloodyMaryEffect(Player player, float intensity)
        {
            // See CalamityPlayerOnHit
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(204, 90, 73),
                new Color(168, 37, 37),
                new Color(120, 28, 28)
            };
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(34, 56, ModContent.BuffType<BloodyMaryBuff>(), CalamityUtils.MinutesToFrames(6), true);
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.BloodMoonStarter).
                AddTile(TileID.Kegs).
                Register();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 9 * player.direction;
            player.itemLocation.Y -= 11;
        }
    }
}

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
    public class BaconOil : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";
        public LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.BaconOil.DripEffect");
        public AlcoholType AlcoholVariant => AlcoholType.BaconOil;
        public Action<Player, float> IVDripAlcoholEffect => ApplyBaconOilEffect;

        private static void ApplyBaconOilEffect(Player player, float intensity)
        {
            // See CalamityPlayer
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
            Item.DefaultToFood(20, 32, ModContent.BuffType<BaconOilBuff>(), CalamityUtils.MinutesToFrames(6), true);
            Item.value = Item.buyPrice(silver: 30); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Blue;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += 1 * player.direction;
            player.itemLocation.Y -= 5;
        }
    }
}

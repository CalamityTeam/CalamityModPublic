using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class MomentumCapacitor : ModItem
    {
        internal const float MomentumChargePerFrame = 0.02f;
        internal const float MaxMomentumCharge = 5.8f; // +580% movemnt speed
        internal const int TotalFadeTime = 16;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Momentum Capacitor");
            Tooltip.SetDefault("While using the Momentum Capacitor,\n" + "your top speed will continuously and uncontrollably increase");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(gold: 36);
            Item.rare = ItemRarityID.Pink;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 2;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override bool? UseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.momentumCapacitorTime = TotalFadeTime;
            modPlayer.momentumCapacitorBoost += Main.rand.NextFloat(0.5f, 3.5f) * MomentumChargePerFrame;
            if (modPlayer.momentumCapacitorBoost >= MaxMomentumCharge)
                modPlayer.momentumCapacitorBoost = MaxMomentumCharge;
            return null;
        }
    }
}

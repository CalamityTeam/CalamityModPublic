using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LeviathanAmbergris : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int ambergrisDashDamage => 350;
        public static float DashSpeedIncrease => 1f;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<RiptideDebuff>()];
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.lAmbergris = true;
            modPlayer.lAmbergrisVisual = !hideVisual;
        }
    }
}

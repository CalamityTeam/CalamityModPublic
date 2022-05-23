using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class TheFirstShadowflame : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("The First Shadowflame");
            Tooltip.SetDefault("It is said that in the past, Prometheus descended from the heavens to grant man fire\n" +
                "If that were true, then it is surely the demons of hell that would have risen from below to do the same\n" +
                "Increases max minions by 1 and minions inflict the Shadowflame debuff on enemies\n" +
                "Grants immunity to Shadowflame");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.shadowMinions = true;
            player.buffImmune[ModContent.BuffType<Shadowflame>()] = true;
        }
    }
}

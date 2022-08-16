using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class ElysianWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Elysian Wings");
            Tooltip.SetDefault("Blessed by the Profaned Flame\n" +
                "Horizontal speed: 9.50\n" +
                "Acceleration multiplier: 2.7\n" +
                "Great vertical speed\n" +
                "Flight time: 240\n" +
                "Temporary immunity to lava and 10% increased movement speed");
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(240, 9.5f, 2.7f);
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.moveSpeed += 0.1f;
            player.lavaMax += 240;
            player.noFallDmg = true;
            modPlayer.elysianFire = true;
            if (hideVisual)
            {
                modPlayer.elysianFire = false;
            }
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }
    }
}

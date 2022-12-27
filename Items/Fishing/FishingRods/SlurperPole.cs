using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class SlurperPole : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.CanFishInLava[Item.type] = true;

            DisplayName.SetDefault("Slurper Pole");
            Tooltip.SetDefault("Can fish in lava.\n" + //Charles Spurgeon quote
                "It is the burning lava of the soul that has a furnace within--a very volcano of grief and sorrow.");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 25;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<SlurperBobber>();
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
        }
    }
}

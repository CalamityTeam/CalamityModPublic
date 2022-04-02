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
            DisplayName.SetDefault("Slurper Pole");
            Tooltip.SetDefault("Can fish in lava.\n" + //Charles Spurgeon quote
                "It is the burning lava of the soul that has a furnace within--a very volcano of grief and sorrow.");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 25;
            item.shootSpeed = 14f;
            item.shoot = ModContent.ProjectileType<SlurperBobber>();
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
        }
    }
}

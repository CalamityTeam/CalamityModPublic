using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
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
			//item.CloneDefaults(2289); //Wooden Fishing Pole
			item.width = 24;
			item.height = 28;
			item.useAnimation = 8;
			item.useTime = 8;
			item.useStyle = 1;
			item.UseSound = SoundID.Item1;
			item.fishingPole = 25;
			item.shootSpeed = 14f;
			item.shoot = ModContent.ProjectileType<SlurperBobber>();
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
			ItemID.Sets.CanFishInLava[item.type] = true;
        }
    }
}

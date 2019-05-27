using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    class Fabsol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fabsol");
            Tooltip.SetDefault("Summons an alicorn\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
			item.rare = 10;
			item.value = Item.buyPrice(3, 0, 0, 0);
			item.expert = true;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = mod.MountType("Fab");
        }
    }
}

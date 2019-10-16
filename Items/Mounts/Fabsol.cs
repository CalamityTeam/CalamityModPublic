using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

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
            item.rare = 9;
            item.value = Item.buyPrice(3, 0, 0, 0);
            item.expert = true;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = ModContent.MountType<AlicornMount>();
        }
    }
}

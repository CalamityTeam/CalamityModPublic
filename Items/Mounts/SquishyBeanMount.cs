using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    class SquishyBeanMount : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious Looking Jelly Bean");
            Tooltip.SetDefault("JELLY BEAN");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.rare = 9;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.expert = true;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = ModContent.MountType<SquishyBean>();
        }
    }
}

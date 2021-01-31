using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class TundraLeash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tundra Leash");
            Tooltip.SetDefault("Summons an angry dog mount");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.UseSound = SoundID.NPCHit56;
            item.noMelee = true;
            item.mountType = ModContent.MountType<AngryDogMount>();
        }
    }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Mounts
{
    public class TundraLeash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tundra Leash");
            Tooltip.SetDefault("Summons a Rimehound mount");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(0, 9, 0, 0);
            Item.UseSound = SoundID.NPCHit56;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<RimehoundMount>();
        }
    }
}

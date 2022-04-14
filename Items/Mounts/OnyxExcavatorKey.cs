using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Mounts
{
    public class OnyxExcavatorKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Excavator Key");
            Tooltip.SetDefault("Summons a drill to drill through the world so you can destroy all the neat world generation\n" +
                "with complete disregard for all the creatures that inhabit these lands. I am sure the EPA and PETA would like\n" +
                "to have a word with you afterwards.\n" +
                "The power of the destruction scales with the highest powered pickaxe in your inventory");
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
            Item.UseSound = SoundID.Item23;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<OnyxExcavator>();
        }
    }
}

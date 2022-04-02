using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class Fabsol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Princess Spirit in a Bottle");
            Tooltip.SetDefault("Summons the spirit of Cirrus, the Drunk Princess, in her alicorn form\n" +
                "Mounting will transform Cirrus, dismounting transforms her back");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = ModContent.MountType<AlicornMount>();

            item.value = Item.buyPrice(platinum: 3);
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().devItem = true;
        }
    }
}

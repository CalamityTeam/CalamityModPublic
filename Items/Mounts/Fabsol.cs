using CalamityMod.Rarities;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item3;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<AlicornMount>();

            Item.value = Item.buyPrice(platinum: 3);
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().devItem = true;
        }
    }
}

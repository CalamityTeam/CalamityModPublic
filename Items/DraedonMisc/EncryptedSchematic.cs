using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematic : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Encrypted Schematic");
            Tooltip.SetDefault("May be deciphered at the Codebreaker if it is upgraded enough");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.maxStack = 999;
        }
    }
}

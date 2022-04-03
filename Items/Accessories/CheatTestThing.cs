using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CheatTestThing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("lul");
            Tooltip.SetDefault("Grants complete invulnerability to almost everything");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = 0; // lul intentionally has zero value
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().lol = true;
        }
    }
}

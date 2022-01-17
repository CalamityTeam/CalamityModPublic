using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CheatTestThing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("lul");
            Tooltip.SetDefault("Grants complete invulnerability to everything");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 0; // lul intentionally has zero value
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().lol = true;
        }
    }
}

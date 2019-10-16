using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class Abaddon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abaddon");
            Tooltip.SetDefault("Makes you immune to Brimstone Flames");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
        }
    }
}

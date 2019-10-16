using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FrostBarrier : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Barrier");
            Tooltip.SetDefault("You will freeze enemies near you when you are struck\n" +
                               "You are immune to the chilled debuff");
        }

        public override void SetDefaults()
        {
            item.defense = 4;
            item.width = 20;
            item.height = 24;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fBarrier = true;
            player.buffImmune[46] = true;
        }
    }
}

using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TheFirstShadowflame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The First Shadowflame");
            Tooltip.SetDefault("It is said that in the past, Prometheus descended from the heavens to grant man fire.\n" +
                "If that were true, then it is surely the demons of hell that would have risen from below to do the same.\n" +
                "Minions inflict shadowflame on enemy hits.");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
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
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.shadowMinions = true;
        }
    }
}

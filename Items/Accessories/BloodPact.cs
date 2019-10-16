using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BloodPact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Pact");
            Tooltip.SetDefault("Doubles your max HP\n" +
                "Allows you to be critically hit 25% of the time");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 8;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodPact = true;
        }
    }
}

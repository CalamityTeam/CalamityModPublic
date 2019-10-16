using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class GiantPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Pearl");
            Tooltip.SetDefault("You have a light aura around you\n" +
                "Enemies within the aura are slowed down\n" +
                "Does not work on bosses");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 32;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.giantPearl = true;
            Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 0.45f, 0.8f, 0.8f);
        }
    }
}

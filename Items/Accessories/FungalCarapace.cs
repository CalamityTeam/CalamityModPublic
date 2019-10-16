using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FungalCarapace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Carapace");
            Tooltip.SetDefault("You emit a mushroom spore explosion when you are hit");
        }

        public override void SetDefaults()
        {
            item.defense = 2;
            item.width = 20;
            item.height = 24;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fCarapace = true;
        }
    }
}

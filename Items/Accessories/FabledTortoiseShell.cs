using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FabledTortoiseShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fabled Tortoise Shell");
            Tooltip.SetDefault("50% reduced movement speed\n" +
                                "Enemies take damage when they hit you\n" +
                                "You move quickly for a short time if you take damage");
        }

        public override void SetDefaults()
        {
            item.defense = 35;
            item.width = 20;
            item.height = 24;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 5;
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fabledTortoise = true;
            player.moveSpeed -= 0.5f;
            player.thorns = 0.25f;
        }
    }
}

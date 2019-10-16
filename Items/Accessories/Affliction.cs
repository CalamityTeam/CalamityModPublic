using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Affliction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Affliction");
            Tooltip.SetDefault("Gives you and all other players on your team +1 life regen,\n" +
                               "+10% max life, 7% damage reduction, 20 defense, and 12% increased damage");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.accessory = true;
            item.expert = true;
            item.rare = 9;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.affliction = true;
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    Main.player[myPlayer].AddBuff(ModContent.BuffType<Afflicted>(), 20, true);
                }
            }
        }
    }
}

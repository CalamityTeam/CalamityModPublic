using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.Accessories
{
    public class MutatedTruffle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutated Truffle");
            Tooltip.SetDefault("Summons a small Old Duke to fight for you\n" +
                               "When below 50% life, it moves much faster");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 26;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.expert = true;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.miniOldDuke = true;
            if (player.whoAmI == Main.myPlayer)
            {
                const int damage = 2000;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<YoungDuke>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, 
                        ModContent.ProjectileType<YoungDuke>(),
                        (int)(damage * (player.allDamage + player.minionDamage - 1f)), 
                        6.5f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

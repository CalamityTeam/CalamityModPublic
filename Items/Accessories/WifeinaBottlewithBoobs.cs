using CalamityMod.CalPlayer;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Items.Accessories
{
    public class WifeinaBottlewithBoobs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rare Elemental in a Bottle");
            Tooltip.SetDefault("Summons a sand elemental to heal you\n" +
                ";D");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sandBoobWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<SandyHealingWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SandyHealingWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<SandElementalHealer>(), (int)(45f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

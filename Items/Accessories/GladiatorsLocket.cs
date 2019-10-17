using CalamityMod.CalPlayer;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Items
{
    public class GladiatorsLocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator's Locket");
            Tooltip.SetDefault("Summons two spirit swords to protect you");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 36;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.defense = 5;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity(); //there might be an upgrade sometime later?
            if (modPlayer.gladiatorSword)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gladiatorSword = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<GladiatorSwords>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<GladiatorSwords>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<GladiatorSword>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GladiatorSword>(), (int)(20f * player.minionDamage), 6f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GladiatorSword2>(), (int)(20f * player.minionDamage), 6f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

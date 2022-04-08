using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
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
            Item.width = 42;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 5;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (player.Calamity().gladiatorSword)
                return false;

            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gladiatorSword = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<GladiatorSwords>()) == -1)
                    player.AddBuff(ModContent.BuffType<GladiatorSwords>(), 3600, true);

                int damage = 30;
                int swordDmg = (int)(damage * player.AverageDamage());
                var source = player.GetProjectileSource_Accessory(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<GladiatorSword>()] < 1)
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<GladiatorSword>(), swordDmg, 2f, Main.myPlayer);
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<GladiatorSword2>(), swordDmg, 2f, Main.myPlayer);
                }
            }
        }
    }
}

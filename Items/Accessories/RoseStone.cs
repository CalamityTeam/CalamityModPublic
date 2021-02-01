using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class RoseStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rose Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
               "Increases max life by 20, life regen by 1, and all damage by 3%\n" +
               "Summons a brimstone elemental to fight for you");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
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
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.6f, 0f, 0.25f);
            player.lifeRegen += 1;
            player.statLifeMax2 += 20;
            player.allDamage += 0.03f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.brimstoneWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<BrimstoneWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), (int)(45 * player.MinionDamage()), 2f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

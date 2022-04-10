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
               "Summons a brimstone elemental to fight for you");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
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
            modPlayer.brimstoneWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetProjectileSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<BrimstoneWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), (int)(60 * player.MinionDamage()), 2f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 60;
                }
            }
        }
    }
}

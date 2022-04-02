using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.rare = ItemRarityID.Purple;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.miniOldDuke = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<MutatedTruffleBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<MutatedTruffleBuff>(), 3600, true);
                }
                const int damage = 1200;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<YoungDuke>()] < 1)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero,
                        ModContent.ProjectileType<YoungDuke>(),
                        (int)(damage * player.MinionDamage()),
                        6.5f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

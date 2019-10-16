using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FungalClump : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Clump");
            Tooltip.SetDefault("Summons a fungal clump to fight for you\n" +
                       "The clump latches onto enemies and steals their life for you");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.fungalClump)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fungalClump = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<FungalClump>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<FungalClump>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<FungalClump>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<FungalClump>(), (int)(10f * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}

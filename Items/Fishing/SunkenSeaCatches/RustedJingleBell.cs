using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class RustedJingleBell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusted Jingle Bell");
            Tooltip.SetDefault("Summons a baby ghost bell light pet\n" +
                "Provides a moderate amount of light while underwater");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.shoot = ModContent.ProjectileType<BabyGhostBell>();
            item.buffType = ModContent.BuffType<BabyGhostBellBuff>();
            item.value = Item.sellPrice(gold: 5);
            item.rare = ItemRarityID.Orange;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}

using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class PlagueCaller : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Caller");
            Tooltip.SetDefault("Summons a baby Plaguebringer pet");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<PlaguebringerBab>();
            item.buffType = ModContent.BuffType<PlaguebringerBabBuff>();
            item.rare = ItemRarityID.Lime;
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

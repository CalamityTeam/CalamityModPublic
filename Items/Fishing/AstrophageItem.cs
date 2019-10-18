using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Fishing
{
    public class AstrophageItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrophage");
            Tooltip.SetDefault("Summons an astrophage to follow you around");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<Astrophage>();
            item.buffType = ModContent.BuffType<AstrophageBuff>();
            item.rare = 5;
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

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Pets
{
    public class ForgottenDragonEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forgotten Dragon Egg");
            Tooltip.SetDefault("Calls Akato, son of Yharon, to your side");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<Akato>();
            item.buffType = ModContent.BuffType<AkatoYharonBuff>();
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
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

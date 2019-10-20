using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Buffs.Pets;

namespace CalamityMod.Items.Pets
{
    public class IbarakiBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hermit's Box of One Hundred Medicines");
            Tooltip.SetDefault("As the ice melts in the springs\n" +
                "And waves wash the old moss’ hair…\n" +
                "Thank you, Goodbye.\n" +
                "While equipped, the player will spawn with full health rather than half.\n" +
                "Summons the Third Sage");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.noMelee = true;
            item.width = 36;
            item.height = 30;
            item.UseSound = SoundID.Item3;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.shoot = ModContent.ProjectileType<ThirdSage>();
            item.buffType = ModContent.BuffType<ThirdSageBuff>();
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

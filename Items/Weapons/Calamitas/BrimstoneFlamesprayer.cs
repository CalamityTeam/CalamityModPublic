using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Calamitas
{
    public class BrimstoneFlamesprayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Havoc's Breath");
        }

        public override void SetDefaults()
        {
            item.damage = 59;
            item.ranged = true;
            item.width = 50;
            item.height = 18;
            item.useTime = 9;
            item.useAnimation = 30;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("BrimstoneFireFriendly");
            item.shootSpeed = 8.5f;
            item.useAmmo = 23;
        }
    }
}

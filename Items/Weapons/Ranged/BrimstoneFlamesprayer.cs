using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
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
            item.shoot = ModContent.ProjectileType<BrimstoneFireFriendly>();
            item.shootSpeed = 8.5f;
            item.useAmmo = 23;
        }
    }
}

using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Murasama : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Murasama");
            Tooltip.SetDefault("There will be blood!\n" +
                "ID and power-level locked\n" +
                "Prove your strength or have the correct user ID to wield this sword");
        }

        public override void SetDefaults()
        {
            item.width = 72;
            item.damage = 999;
            item.crit += 30;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.useTime = 5;
            item.knockBack = 6.5f;
            item.autoReuse = false;
            item.height = 78;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Murasama");
            item.shootSpeed = 24f;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityWorld.downedYharon || player.name == "Sam" || player.name == "Samuel Rodrigues";
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Arbalest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arbalest");
            Tooltip.SetDefault("Fires volleys of high-speed arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.ranged = true;
            item.width = 58;
            item.height = 22;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 3; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                int proj = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[proj].extraUpdates += i;
                Main.projectile[proj].noDropItem = true;
            }
            return false;
        }
    }
}

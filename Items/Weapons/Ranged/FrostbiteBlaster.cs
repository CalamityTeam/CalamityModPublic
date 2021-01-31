using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FrostbiteBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostbite Blaster");
            Tooltip.SetDefault("Fires a spread of icicles");
        }
        public override void SetDefaults()
        {
            item.damage = 31;
            item.ranged = true;
            item.width = 54;
            item.height = 30;
            item.useTime = 7;
            item.useAnimation = 21;
            item.reuseDelay = 54;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTurn = false;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.useAmmo = AmmoID.Bullet;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = ProjectileID.Blizzard;
            item.shootSpeed = 9f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Main.PlaySound(SoundID.Item36, position);

            type = ProjectileID.Blizzard;
            for (int i = 0; i < 2; i++)
            {
                float newSpeedX = speedX + Main.rand.Next(-40, 41) * 0.06f;
                float newSpeedY = speedY + Main.rand.Next(-40, 41) * 0.06f;
                int p = Projectile.NewProjectile(position.X, position.Y, newSpeedX, newSpeedY, type, damage, knockBack, player.whoAmI);
                Main.projectile[p].magic = false;
                Main.projectile[p].ranged = true;
            }
            return true;
        }
    }
}

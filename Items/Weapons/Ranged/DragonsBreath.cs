using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DragonsBreath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon's Breath");
            Tooltip.SetDefault("80% chance to not consume ammo\n" +
				"Fires a spread of 8 bullets\n" +
				"Converts musket balls into exploding dragonfire bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 200;
            item.ranged = true;
            item.width = 64;
            item.height = 28;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6.5f;
			item.value = CalamityGlobalItem.Rarity15BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Violet;
			item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 8; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<DragonBurst>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 80)
                return false;
            return true;
        }
    }
}

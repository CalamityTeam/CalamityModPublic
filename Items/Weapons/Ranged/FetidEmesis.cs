using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FetidEmesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fetid Emesis");
            Tooltip.SetDefault("Has a chance to release rotten chunks instead of bullets.");
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.ranged = true;
            item.width = 76;
            item.height = 46;
            item.useTime = item.useAnimation = 6;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
			item.rare = 10;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.NextBool(8))
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 0.45f,
                    ModContent.ProjectileType<EmesisGore>(), damage, knockBack, player.whoAmI);
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(position, 10, 10, 27);
                    dust.velocity = Vector2.Normalize(new Vector2(speedX, speedY)).RotatedByRandom(MathHelper.ToRadians(15f));
                    dust.noGravity = true;
                }
				if (player.Calamity().soundCooldown <= 0)
				{
					// WoF vomit sound.
					Main.PlaySound(SoundID.NPCKilled, (int)position.X, (int)position.Y, 13, 0.5f, 0f);
					player.Calamity().soundCooldown = 120;
				}
                return false;
            }
            return true;
        }
    }
}

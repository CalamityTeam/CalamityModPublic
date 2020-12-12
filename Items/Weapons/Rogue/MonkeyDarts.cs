using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MonkeyDarts : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monkey Darts");
            Tooltip.SetDefault("Stealth strikes throw 3 bouncing darts at high speed\n" + "'Perfect for popping'");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 150;
            item.knockBack = 4;
            item.crit = 18;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.maxStack = 999;
            item.width = 27;
            item.height = 27;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.value = Item.buyPrice(0, 0, 4, 0);
            item.rare = 7;
            item.shootSpeed = 8f;
            item.shoot = ModContent.ProjectileType<MonkeyDart>();
            item.autoReuse = true;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //Checks if stealth is avalaible to shoot a spread of 3 darts
            if (player.Calamity().StealthStrikeAvailable())
            {
                float spread = 7;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX * 1.25f, speedY * 1.25f).RotatedBy(MathHelper.ToRadians(spread));
                    int p = Projectile.NewProjectile(position, perturbedspeed, ModContent.ProjectileType<MonkeyDart>(), damage, knockBack, player.whoAmI, 1);
					if (p.WithinBounds(Main.maxProjectiles))
						Main.projectile[p].Calamity().stealthStrike = true;
                    spread -= 7;
                }
                return false;
            }

            else
            {
                return true;
            }
        }

    }
}

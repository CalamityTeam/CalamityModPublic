using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Valediction : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valediction");
            Tooltip.SetDefault("Throws a homing reaper scythe\n" +
			"Stealth strikes spawn razorblade typhoons on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 80;
            item.damage = 263;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<ValedictionBoomerang>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
				if (stealth.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class UtensilPoker : RogueWeapon
    {
		private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Utensil Poker");
            Tooltip.SetDefault("Space chickens, that is all.\n" +
				"Fires random utensils in bursts of three\n" +
                "Grants Well Fed on enemy hits\n" +
                "Stealth strikes launch an additional butcher knife");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.height = 66;
            item.damage = 405;
            item.Calamity().rogue = true;
            item.knockBack = 8f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.reuseDelay = 15;
            item.useAnimation = 45;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.shoot = ModContent.ProjectileType<Fork>();
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable() && counter == 0)
            {
                int stealth = Projectile.NewProjectile(position.X, position.Y, speedX * 1.2f, speedY * 1.2f, ModContent.ProjectileType<ButcherKnife>(), (int)(damage * 1.4), knockBack, player.whoAmI);
                Main.projectile[stealth].Calamity().stealthStrike = true;
            }
			int utensil = item.shoot;
			double dmgMult = 1D;
			float kbMult = 1f;
			switch (Main.rand.Next(3))
			{
				case 0:
					utensil = item.shoot;
					dmgMult = 1.1;
					kbMult = 2f;
					break;
				case 1:
					utensil = ModContent.ProjectileType<Knife>();
					dmgMult = 1.2;
					kbMult = 1f;
					break;
				case 2:
					utensil = ModContent.ProjectileType<CarvingFork>();
					dmgMult = 1D;
					kbMult = 1f;
					break;
                default:
                    break;
			}
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), utensil, (int)(damage * dmgMult), knockBack * kbMult, player.whoAmI);

			counter++;
			if (counter >= 3)
				counter = 0;
            return false;
        }
    }
}

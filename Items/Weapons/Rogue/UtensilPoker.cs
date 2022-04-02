using CalamityMod.CalPlayer;
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
                "Stealth strikes replace any utensil with a powerful butcher knife");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.height = 66;
            item.damage = 333;
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
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.shoot = ModContent.ProjectileType<Fork>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer mp = player.Calamity();

            if (mp.StealthStrikeAvailable())
            {
                int stealthDamage = damage * 2;
                float stealthSpeedMult = 1.4f;
                Vector2 stealthVelocity = new Vector2(speedX, speedY) * stealthSpeedMult;
                int stealth = Projectile.NewProjectile(position, stealthVelocity, ModContent.ProjectileType<ButcherKnife>(), stealthDamage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            else
            {
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
            }

            counter++;
            if (counter == 3)
                counter = 0;
            return false;
        }
    }
}

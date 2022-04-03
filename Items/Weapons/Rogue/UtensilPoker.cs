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
            Item.width = 44;
            Item.height = 66;
            Item.damage = 333;
            Item.Calamity().rogue = true;
            Item.knockBack = 8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.reuseDelay = 15;
            Item.useAnimation = 45;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<Fork>();
            Item.shootSpeed = 12f;
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
                int utensil = Item.shoot;
                double dmgMult = 1D;
                float kbMult = 1f;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        utensil = Item.shoot;
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

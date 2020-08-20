using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StellarKnife : RogueWeapon
    {
        int knifeCount = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Knife");
            Tooltip.SetDefault("Throws knives that stop middair and then home into enemies\n" +
                               "Stealth strikes throw a volley of " + knifeCount + " knives in a spread\n" +
                               "Za Warudo");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.damage = 50;
            item.crit += 4;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 9;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<StellarKnifeProj>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[ModContent.ProjectileType<StellarKnifeProj>()] < 10)
            {
                int spread = 20;
                for (int i = 0; i < knifeCount; i++)
                {
                    speedX *= 0.9f;
                    Vector2 perturbedspeed = new Vector2(speedX, speedY + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                    Projectile.NewProjectile(position, perturbedspeed, type, damage, knockBack, player.whoAmI, 1f, i % 5 == 0 ? 1f : 0f);
                    spread -= Main.rand.Next(1, 3);
                }
                return false;
            }
            return true;
        }
    }
}

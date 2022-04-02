using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StellarKnife : RogueWeapon
    {
        int knifeCount = 10;
        int knifeLimit = 20;
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
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 9;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<StellarKnifeProj>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 4;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[item.shoot] < knifeLimit)
            {
                damage = (int)(damage * 1.1f);

                int knifeAmt = knifeCount;
                if ((player.ownedProjectileCounts[item.shoot] + knifeCount) >= knifeLimit)
                    knifeAmt = knifeLimit - player.ownedProjectileCounts[item.shoot];
                if (knifeAmt <= 0)
                {
                    int knife = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                    if (knife.WithinBounds(Main.maxProjectiles))
                        Main.projectile[knife].Calamity().stealthStrike = true;
                }

                int spread = 20;
                for (int i = 0; i < knifeCount; i++)
                {
                    speedX *= 0.9f;
                    Vector2 perturbedspeed = new Vector2(speedX, speedY + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                    int knife2 = Projectile.NewProjectile(position, perturbedspeed, type, damage, knockBack, player.whoAmI, 1f, i % 5 == 0 ? 1f : 0f);
                    if (knife2.WithinBounds(Main.maxProjectiles))
                        Main.projectile[knife2].Calamity().stealthStrike = true;
                    spread -= Main.rand.Next(1, 3);
                }
                return false;
            }
            return true;
        }
    }
}

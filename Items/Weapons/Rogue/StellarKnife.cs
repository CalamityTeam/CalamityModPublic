using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StellarKnife : ModItem
    {
        int knifeCount = 10;
        int knifeLimit = 20;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Knife");
            Tooltip.SetDefault("Throws knives that stop middair and then home into enemies\n" +
                               "Stealth strikes throw a volley of " + knifeCount + " knives in a spread\n" +
                               "Za Warudo");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.damage = 50;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 9;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<StellarKnifeProj>();
            Item.shootSpeed = 10f;
            Item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[Item.shoot] < knifeLimit)
            {
                damage = (int)(damage * 1.1f);

                int knifeAmt = knifeCount;
                if ((player.ownedProjectileCounts[Item.shoot] + knifeCount) >= knifeLimit)
                    knifeAmt = knifeLimit - player.ownedProjectileCounts[Item.shoot];
                if (knifeAmt <= 0)
                {
                    int knife = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                    if (knife.WithinBounds(Main.maxProjectiles))
                        Main.projectile[knife].Calamity().stealthStrike = true;
                }

                int spread = 20;
                for (int i = 0; i < knifeCount; i++)
                {
                    velocity.X *= 0.9f;
                    Vector2 perturbedspeed = new Vector2(velocity.X, velocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                    int knife2 = Projectile.NewProjectile(source, position, perturbedspeed, type, damage, knockback, player.whoAmI, 1f, i % 5 == 0 ? 1f : 0f);
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

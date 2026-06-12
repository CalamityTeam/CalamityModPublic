using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StellarKnife : RogueWeapon
    {
        int knifeCount = 10;
        int knifeLimit = 20;
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.damage = 69;
            Item.crit = 4;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<StellarKnifeProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[Item.shoot] < knifeLimit)
            {
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

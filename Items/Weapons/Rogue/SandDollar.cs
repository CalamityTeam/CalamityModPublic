using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SandDollar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Dollar");
            Tooltip.SetDefault("Stacks up to 2\n" +
            "Stealth strikes throw 2 long ranged sand dollars that explode into coral shards on enemy hits");
            SacrificeTotal = 2;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.damage = 26;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 2;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice / 2; // Stacks to 2
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SandDollarProj>();
            Item.shootSpeed = 14f;
        }

        public override bool CanUseItem(Player player)
        {
            int UseMax = Item.stack - 1;

            if (player.ownedProjectileCounts[Item.shoot] > UseMax && !player.Calamity().StealthStrikeAvailable())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		public override float StealthDamageMultiplier => 0.75f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 2;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 perturbedspeed = new Vector2(velocity.X + Main.rand.Next(-2,3), velocity.Y + Main.rand.Next(-2,3)).RotatedBy(MathHelper.ToRadians(spread));
                    int stealth = Projectile.NewProjectile(source, position.X, position.Y, perturbedspeed.X * 1.5f, perturbedspeed.Y * 1.5f, ModContent.ProjectileType<SandDollarStealth>(), damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                    spread -= Main.rand.Next(1,3);
                }
                return false;
            }
            return true;
        }
    }
}

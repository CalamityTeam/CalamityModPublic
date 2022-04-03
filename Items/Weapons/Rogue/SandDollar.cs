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
        }

        public override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.damage = 26;
            Item.DamageType = DamageClass.Throwing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 2;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SandDollarProj>();
            Item.shootSpeed = 14f;
            Item.Calamity().rogue = true;
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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 2;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-2,3), speedY + Main.rand.Next(-2,3)).RotatedBy(MathHelper.ToRadians(spread));
                    int stealth = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X * 1.5f, perturbedspeed.Y * 1.5f, ModContent.ProjectileType<SandDollarStealth>(), Math.Max((int)(damage * 0.75), 1), knockBack, player.whoAmI);
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

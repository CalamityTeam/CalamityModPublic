using Terraria.DataStructures;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Kylie : ModItem
    {
        public static float Speed = 11f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kylie");
            Tooltip.SetDefault("Stealth strikes throws three short ranged kylies instead of a single long range one\n" + "'Also known as Dowak'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 63;
            Item.knockBack = 12;
            Item.DamageType = DamageClass.Throwing;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.width = 32;
            Item.height = 46;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = Speed;
            Item.shoot = ModContent.ProjectileType<KylieBoomerang>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = RogueDamageClass.Instance;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer p = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range
            if (p.StealthStrikeAvailable())
            {
                int spread = 10;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = velocity.RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(source, position, perturbedspeed, ModContent.ProjectileType<KylieBoomerang>(), Math.Max(damage / 3, 1), knockback / 3f, player.whoAmI, 0f, 1f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= 10;
                }
                return false;
            }
            return true;
        }
    }
}

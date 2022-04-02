using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Kylie : RogueWeapon
    {
        public static float Speed = 11f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kylie");
            Tooltip.SetDefault("Stealth strikes throws three short ranged kylies instead of a single long range one\n" + "'Also known as Dowak'");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 63;
            item.knockBack = 12;
            item.thrown = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.useTime = 25;
            item.useAnimation = 25;
            item.width = 32;
            item.height = 46;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.shootSpeed = Speed;
            item.shoot = ModContent.ProjectileType<KylieBoomerang>();
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 16;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer p = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range
            if (p.StealthStrikeAvailable())
            {
                int spread = 10;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position, perturbedspeed, ModContent.ProjectileType<KylieBoomerang>(), Math.Max(damage / 3, 1), knockBack / 3f, player.whoAmI, 0f, 1f);
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

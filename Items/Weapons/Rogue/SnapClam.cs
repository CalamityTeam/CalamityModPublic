using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SnapClam : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snap Clam");
            Tooltip.SetDefault("Can latch on enemies and deal damage over time\n" +
            "Stealth strikes throw five clams at once that cause increased damage over time");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 16;
            item.damage = 14;
            item.thrown = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<SnapClamProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 3;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position, perturbedspeed, ModContent.ProjectileType<SnapClamStealth>(), Math.Max(damage / 5, 1), knockBack / 5f, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= Main.rand.Next(1,3);
                }
                return false;
            }
            return true;
        }
    }
}

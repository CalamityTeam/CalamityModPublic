using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class HeavenfallenStardisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenfallen Stardisk");
            Tooltip.SetDefault("Throws a stardisk upwards which then launches itself towards your mouse cursor,\n" +
                               "explodes into several astral energy bolts if the thrower is moving vertically when throwing it and during its impact\n" +
                               "Stealth strikes rain astral energy bolts from the sky");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.height = 48;
            item.damage = 115;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<HeavenfallenStardiskBoomerang>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position.X, position.Y, 0f, -10f, type, (int)(damage * 1.25f), knockBack, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, 0f, -10f, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}

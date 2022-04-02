using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CraniumSmasher : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cranium Smasher");
            Tooltip.SetDefault("Throws disks that roll on the ground, occasionally launches an explosive disk\n" +
            "Stealth strikes launch an explosive disk that can pierce several enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.damage = 120;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<CraniumSmasherProj>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                damage = (int)(damage * 1.75f);
                type = ModContent.ProjectileType<CraniumSmasherStealth>();
            }
            else if (Main.rand.NextBool(5))
            {
                damage = (int)(damage * 1.25f);
                type = ModContent.ProjectileType<CraniumSmasherExplosive>();
            }
            else
            {
                type = ModContent.ProjectileType<CraniumSmasherProj>();
            }
            int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CraniumSmasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cranium Smasher");
            Tooltip.SetDefault("Throws disks that roll on the ground, occasionally launches an explosive disk\n" +
            "Stealth strikes launch an explosive disk that can pierce several enemies");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 120;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<CraniumSmasherProj>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            int proj = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

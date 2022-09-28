using Terraria.DataStructures;
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
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<CraniumSmasherProj>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override float StealthDamageMultiplier => 1.75f;

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (player.Calamity().StealthStrikeAvailable())
            {
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
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

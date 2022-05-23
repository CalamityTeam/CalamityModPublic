using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RadiantStar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Star");
            Tooltip.SetDefault("Throws daggers that explode and split after a while\n" +
                "Stealth strike splits more with a devastating explosion and sucks enemies in");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.damage = 55; //33
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 12;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 48;
            Item.value = Item.buyPrice(0, 95, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<RadiantStarKnife>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 8;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Prismalline>().
                AddIngredient<AstralBar>(10).
                AddIngredient<Stardust>(15).
                AddIngredient(ItemID.FallenStar, 10).
                AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}

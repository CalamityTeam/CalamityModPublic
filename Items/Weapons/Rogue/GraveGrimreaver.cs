using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GraveGrimreaver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grave Grimreaver");
            Tooltip.SetDefault("Hurls a cursed scythe which homes in\n"+
            "The scythe summons skulls as it flies and explodes into bats on hit\n"+
            "Stealth strikes spawn a flood of bats and falling skulls\n"+
            "Inflicts cursed flames and confusion\n"+
            "'A dapper skeleton's weapon of choice'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.damage = 25;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 51;
            Item.useTime = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 68;
            Item.shoot = ModContent.ProjectileType<GraveGrimreaverProjectile>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.Calamity().donorItem = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                damage = (int)(damage * 1.1);
                int proj = Projectile.NewProjectile(source, position, velocity * 1.1f, type, damage / 2, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Sickle, 1).
                AddRecipeGroup("TombstonesGroup").
                AddIngredient(ItemID.Bone, 50).
                AddIngredient(ItemID.CursedFlame, 5).
                AddIngredient(ItemID.SoulofFright, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

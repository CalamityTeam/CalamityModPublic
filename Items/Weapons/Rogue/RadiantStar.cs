using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class RadiantStar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Star");
            Tooltip.SetDefault("Throws daggers that explode and split after a while\n" +
                "Stealth strike splits more with a devastating explosion and sucks enemies in");
        }

        public override void SafeSetDefaults()
        {
            item.width = 52;
            item.damage = 55; //33
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 12;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 48;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.shoot = ModContent.ProjectileType<RadiantStarKnife>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 8;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Prismalline>());
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 15);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

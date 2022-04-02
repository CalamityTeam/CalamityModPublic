using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class TitaniumShuriken : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Shuriken");
            Tooltip.SetDefault("Stealth strikes act like a boomerang that spawns clones on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 37;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 9;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 38;
            item.maxStack = 999;
            item.value = 2000;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<TitaniumShurikenProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                    Main.projectile[stealth].aiStyle = -1;
                    Main.projectile[stealth].extraUpdates = 1;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TitaniumBar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}

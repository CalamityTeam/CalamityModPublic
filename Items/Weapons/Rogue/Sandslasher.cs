using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Sandslasher : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandslasher");
            Tooltip.SetDefault("Throws a huge shuriken made out of fused sand unaffected by gravity which slowly accelerates horizontally\n"
                              +"It does more damage depending on how fast it goes horizontally and how long it has been flying for\n"
                              +"Stealth strikes periodically release sand clouds");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 115;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<SandslasherProj>();
            item.shootSpeed = 7f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GrandScale>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 6);
            recipe.AddRecipeGroup("AnyGoldBar", 10);
            recipe.AddIngredient(ItemID.HardenedSand, 25);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

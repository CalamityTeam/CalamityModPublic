using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SludgeSplotch : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Splotch");
            Tooltip.SetDefault("Slows non-boss enemies slightly\n" +
                "Stealth strikes cause the main ball of sludge to split on hit, inflicting more damage");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.damage = 30;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 1f;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.sellPrice(0, 0, 0, 40);
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<SludgeSplotchProj1>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Main.PlaySound(SoundID.NPCKilled, (int)player.position.X, (int)player.position.Y, 9, 2, 0);

            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX * 1.2f, speedY * 1.2f, type, damage, knockBack * 3, player.whoAmI, 0f, 1f);
				if (p.WithinBounds(Main.maxProjectiles))
					Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EbonianGel>(), 15);
            recipe.AddRecipeGroup("Boss2Material");
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}

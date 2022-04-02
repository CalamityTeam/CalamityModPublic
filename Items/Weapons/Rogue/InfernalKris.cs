using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class InfernalKris : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Kris");
            Tooltip.SetDefault("Throws a burning dagger that starts spinning after travelling a short distance, inflicting additional damage while spinning\n" +
                "Stealth strikes cause the dagger to be engulfed in flames, exploding on contact with walls and enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.damage = 24;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 5, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<InfernalKrisProjectile>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar);
            recipe.AddIngredient(ItemID.Obsidian, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}

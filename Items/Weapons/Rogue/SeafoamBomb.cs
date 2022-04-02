using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SeafoamBomb : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bomb");
            Tooltip.SetDefault(@"Throws a bomb that explodes into a bubble which deals extra damage to enemies
Stealth strikes are faster and explode into 5 bubbles");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 44;
            item.damage = 12;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<SeafoamBombProj>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bomb, 25);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX + speedX / 3, speedY + speedY / 3), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

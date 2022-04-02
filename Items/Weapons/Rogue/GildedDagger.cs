using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GildedDagger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gilded Dagger");
            Tooltip.SetDefault("Throws a shiny blade that ricochets towards another enemy on hit\n" +
                "Stealth strikes cause the blade to home in after ricocheting, with each ricochet dealing 20% more damage\n" +
                "Stealth strikes also have increased piercing");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.damage = 14;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 26;
            item.maxStack = 1;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<GildedDaggerProj>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    Main.projectile[p].penetrate = 4;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldBar, 12);
            recipe.AddIngredient(ItemID.ThrowingKnife, 250);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}

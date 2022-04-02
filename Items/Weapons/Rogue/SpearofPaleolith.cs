using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofPaleolith : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear of Paleolith");
            Tooltip.SetDefault("Throws an ancient spear that shatters enemy armor\n" +
                "Spears rain fossil shards as they travel\n" +
                "Stealth strikes travel slower but further, raining more fossil shards");
        }

        public override void SafeSetDefaults()
        {
            item.width = 54;
            item.damage = 65;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 27;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<SpearofPaleolithProj>();
            item.shootSpeed = 35f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stabDevice = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stabDevice.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stabDevice].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}

using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Equanimity : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Equanimity");
            Tooltip.SetDefault("Throws a dark/light boomerang that confuses enemies\n" +
                "The boomerang will create light shards upon hitting enemies when thrown out, and will fire homing dark shards when returning\n" +
                "Stealth strikes cause the boomerang to create both dark and light shards whenever one type would be created");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 40;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 36;
            item.maxStack = 1;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<EquanimityProj>();
            item.shootSpeed = 30f;
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
            recipe.AddIngredient(ItemID.Flamarang);
            recipe.AddIngredient(ItemID.IceBoomerang);
            recipe.AddIngredient(ItemID.LightShard);
            recipe.AddIngredient(ItemID.DarkShard);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class IronFrancisca : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Francisca");
            Tooltip.SetDefault("The franciscas do more damage for a short time when initially thrown\n" +
                               "Stealth strikes pierce infinitely");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 7;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.height = 36;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 1, 0);
            item.rare = ItemRarityID.White;
            item.shoot = ModContent.ProjectileType<IronFranciscaProj>();
            item.shootSpeed = 12f;
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
            recipe.AddIngredient(ItemID.Wood);
            recipe.anyWood = true;
            recipe.AddIngredient(ItemID.IronBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}

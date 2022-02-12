using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StormfrontRazor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Razor");
            Tooltip.SetDefault("Throws a throwing knife that leaves sparks as it travels.\n" +
                               "Stealth strikes cause the knife to be faster and leave a huge shower of sparks as it travels");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.useAnimation = 15;
            item.useTime = 15;
            item.damage = 50;
            item.knockBack = 7f;
            item.shoot = ModContent.ProjectileType<StormfrontRazorProjectile>();
            item.shootSpeed = 7f;
            item.Calamity().rogue = true;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 8;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Cinquedea>());
            recipe.AddRecipeGroup("AnyMythrilBar", 6);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 4);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddIngredient(ModContent.ItemType<StormlionMandible>(), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.6f, ModContent.ProjectileType<StormfrontRazorProjectile>(), (int)(damage * 1.1f), knockBack, player.whoAmI, 0, 40f);
				if (p.WithinBounds(Main.maxProjectiles))
					Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            else
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<StormfrontRazorProjectile>(), damage, knockBack, player.whoAmI, 0, 1);
                return false;
            }
        }
    }
}

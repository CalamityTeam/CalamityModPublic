using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AMR : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-materiel Rifle");
            Tooltip.SetDefault("Fires a .50 caliber sniper round that rips apart enemy defense and DR\n" +
                "If you crit the target a second swarm of bullets will fire near the target");
        }

        public override void SetDefaults()
        {
            item.damage = 7000;
            item.ranged = true;
            item.width = 76;
            item.height = 30;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 9.5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire");
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Shroomer>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<AMRShot>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}

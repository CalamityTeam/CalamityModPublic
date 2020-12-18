using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HandheldTank : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Handheld Tank");
        }

        public override void SetDefaults()
        {
            item.width = 110;
            item.height = 46;
            item.ranged = true;
            item.damage = 1000;
            item.knockBack = 16f;
            item.useTime = 71;
            item.useAnimation = 71;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TankCannon");
            item.noMelee = true;

            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;

            item.shoot = ModContent.ProjectileType<HandheldTankShell>();
            item.shootSpeed = 6f;
            item.useAmmo = AmmoID.Rocket;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 15;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<HandheldTankShell>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-33, 0);

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Shroomer>());
            r.AddIngredient(ItemID.IronBar, 50);
            r.anyIronBar = true;
            r.AddIngredient(ModContent.ItemType<DivineGeode>(), 5);
            r.AddIngredient(ItemID.TigerSkin);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}

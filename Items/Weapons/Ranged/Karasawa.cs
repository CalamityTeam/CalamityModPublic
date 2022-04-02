using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Karasawa : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Karasawa");
            Tooltip.SetDefault("...This is heavy... too heavy.");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.height = 44;
            item.ranged = true;
            item.damage = 900;
            item.knockBack = 12f;
            item.useTime = 52;
            item.useAnimation = 52;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/MechGaussRifle");
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.shoot = ModContent.ProjectileType<KarasawaShot>();
            item.shootSpeed = 1f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, item, 5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }
            Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<KarasawaShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 5);

            return false;
        }

        // Disable vanilla ammo consumption
        public override bool ConsumeAmmo(Player player)
        {
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.LargeRuby);
            r.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
            r.AddIngredient(ModContent.ItemType<DubiousPlating>(), 25);
            r.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            r.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            r.AddTile(ModContent.TileType<CosmicAnvil>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}

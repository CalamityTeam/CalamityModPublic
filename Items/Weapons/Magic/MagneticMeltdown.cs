using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class MagneticMeltdown : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnetic Meltdown");
            Tooltip.SetDefault("Fires a spread of magnetic orbs");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.magic = true;
            item.mana = 25;
            item.width = 78;
            item.height = 78;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MagneticOrb>();
            item.shootSpeed = 12f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 4; ++index)
            {
                float SpeedX = speedX + Main.rand.NextFloat(-2.5f, 2.5f);
                float SpeedY = speedY + Main.rand.NextFloat(-2.5f, 2.5f);
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 1f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ItemID.SpectreStaff);
            recipe.AddIngredient(ItemID.MagnetSphere);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

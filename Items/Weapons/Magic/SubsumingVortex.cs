using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class SubsumingVortex : ModItem
    {
        public const int MaxVortexCount = 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subsuming Vortex");
            Tooltip.SetDefault("Fires a giant slow-moving vortex\n" +
                               "When an enemy is nearby, the vortex releases tentacles that redirect towards the enemy.\n" +
                               "After some time, the vortex slows down, charges, and eventually explodes.\n" +
                               $"Only {MaxVortexCount} vortexes can exist at once.");
        }

        public override void SetDefaults()
        {
            item.damage = 480;
            item.magic = true;
            item.mana = 20;
            item.width = 38;
            item.height = 48;
            item.UseSound = SoundID.Item84;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EnormousConsumingVortex>();
            item.shootSpeed = 7f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(19f, 22f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < MaxVortexCount;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AuguroftheElements>());
            recipe.AddIngredient(ModContent.ItemType<EventHorizon>());
            recipe.AddIngredient(ModContent.ItemType<TearsofHeaven>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

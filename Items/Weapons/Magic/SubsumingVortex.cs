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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subsuming Vortex");
            Tooltip.SetDefault("Releases a gigantic, slow-moving vortex\n" +
                               "The vortex releases exo tentacles that thrash at nearby enemies\n" +
                               "After a few seconds the vortex slows down, becomes unstable, and explodes");
        }

        public override void SetDefaults()
        {
            item.damage = 300;
            item.magic = true;
            item.mana = 78;
            item.width = 38;
            item.height = 48;
            item.UseSound = SoundID.Item84;
            item.useTime = item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = Item.buyPrice(platinum: 2, gold: 50);
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EnormousConsumingVortex>();
            item.shootSpeed = 7f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow"));
        }

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

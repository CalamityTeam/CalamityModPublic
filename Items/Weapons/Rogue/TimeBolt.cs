using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TimeBolt : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Time Bolt");
            Tooltip.SetDefault("There should be no boundary to human endeavor.");
        }

        public override void SafeSetDefaults()
        {
            item.width = 24;
            item.height = 46;
            item.damage = 720;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
            item.shoot = ModContent.ProjectileType<TimeBoltKnife>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmicKunai>());
            recipe.AddIngredient(ItemID.FastClock);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

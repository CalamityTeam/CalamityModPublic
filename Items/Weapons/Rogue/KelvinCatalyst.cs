using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KelvinCatalyst : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kelvin Catalyst");
            Tooltip.SetDefault("Throws an icy blade that splits into multiple ice stars on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 100;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 30;
            item.useStyle = 1;
            item.useTime = 30;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.height = 20;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.shoot = ModContent.ProjectileType<Projectiles.KelvinCatalyst>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 21;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "IceStar", 200);
            recipe.AddIngredient(ItemID.SoulofMight, 30);
            recipe.AddIngredient(null, "CryoBar", 20);
            recipe.AddIngredient(null, "EssenceofEleum", 10);
            recipe.AddIngredient(null, "Avalanche", 2);
            recipe.AddIngredient(null, "BittercoldStaff", 2);
            recipe.AddIngredient(null, "EffluviumBow", 2);
            recipe.AddIngredient(null, "GlacialCrusher", 2);
            recipe.AddIngredient(null, "Icebreaker", 2);
            recipe.AddIngredient(null, "SnowstormStaff", 2);
            recipe.AddIngredient(null, "SoulofCryogen", 2);
            recipe.AddIngredient(null, "FrostFlare", 2);
            recipe.AddIngredient(ItemID.FrostCore, 2);
            recipe.AddIngredient(null, "CryoStone");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

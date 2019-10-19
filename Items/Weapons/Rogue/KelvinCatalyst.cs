using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
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
            item.shoot = ModContent.ProjectileType<KelvinCatalyst>();
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
            recipe.AddIngredient(ModContent.ItemType<IceStar>(), 200);
            recipe.AddIngredient(ItemID.SoulofMight, 30);
            recipe.AddIngredient(ModContent.ItemType<CryoBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Avalanche>(), 2);
            recipe.AddIngredient(ModContent.ItemType<BittercoldStaff>(), 2);
            recipe.AddIngredient(ModContent.ItemType<EffluviumBow>(), 2);
            recipe.AddIngredient(ModContent.ItemType<GlacialCrusher>(), 2);
            recipe.AddIngredient(ModContent.ItemType<Icebreaker>(), 2);
            recipe.AddIngredient(ModContent.ItemType<SnowstormStaff>(), 2);
            recipe.AddIngredient(ModContent.ItemType<SoulofCryogen>(), 2);
            recipe.AddIngredient(ModContent.ItemType<FrostFlare>(), 2);
            recipe.AddIngredient(ItemID.FrostCore, 2);
            recipe.AddIngredient(ModContent.ItemType<CryoStone>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

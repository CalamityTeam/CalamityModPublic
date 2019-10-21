using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Oracle : ModItem
    {
        public static int BaseDamage = 480;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Oracle");
            Tooltip.SetDefault("Emits an aura of red lightning\nFires auric orbs when supercharged\nHitting enemies charges the yoyo\n'Gaze into the past, the present, the future... and the circumstances of your inevitable demise'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Kraken);

            item.width = 54;
            item.height = 42;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = 4f;
            item.useTime = 20;
            item.useAnimation = 20;
            item.autoReuse = true;

            item.useStyle = 5;
            item.channel = true;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(2, 50, 0, 0);

            item.shoot = ModContent.ProjectileType<OracleYoyo>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddIngredient(ModContent.ItemType<TheObliterator>());
            r.AddIngredient(ModContent.ItemType<Lacerator>());
            r.AddIngredient(ModContent.ItemType<Verdant>());
            r.AddIngredient(ModContent.ItemType<Chaotrix>());
            r.AddIngredient(ModContent.ItemType<Quagmire>());
            r.AddIngredient(ModContent.ItemType<Shimmerspark>());
            r.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            r.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            r.AddIngredient(ModContent.ItemType<DarksunFragment>(), 5);
            r.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            r.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 3);
            r.AddIngredient(ModContent.ItemType<AuricOre>(), 25);
            r.AddRecipe();
        }
    }
}

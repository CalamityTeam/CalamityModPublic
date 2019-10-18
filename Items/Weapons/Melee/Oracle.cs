using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            r.AddTile(null, "DraedonsForge");
            r.AddIngredient(null, "TheObliterator");
            r.AddIngredient(null, "Lacerator");
            r.AddIngredient(null, "Verdant");
            r.AddIngredient(null, "Chaotrix");
            r.AddIngredient(null, "Quagmire");
            r.AddIngredient(null, "Shimmerspark");
            r.AddIngredient(null, "NightmareFuel", 5);
            r.AddIngredient(null, "EndothermicEnergy", 5);
            r.AddIngredient(null, "CosmiliteBar", 5);
            r.AddIngredient(null, "DarksunFragment", 5);
            r.AddIngredient(null, "Phantoplasm", 5);
            r.AddIngredient(null, "HellcasterFragment", 3);
            r.AddIngredient(null, "AuricOre", 25);
            r.AddRecipe();
        }
    }
}

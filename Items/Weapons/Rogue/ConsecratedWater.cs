using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ConsecratedWater : RogueWeapon
    {
        public const int BaseDamage = 48;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Consecrated Water");
            Tooltip.SetDefault("The bottle is surprisingly dusty\n" +
							   "Throws a holy flask of water that explodes into a sacred flame pillar on death\n" +
                               "The pillar is destroyed if there's no tiles below it\n" +
                               "Stealth strikes create three flame pillars instead of one on impact");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.width = 22;
            item.height = 24;
            item.useAnimation = 29;
            item.useTime = 29;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ConsecratedWaterProjectile>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float strikeValue = player.Calamity().StealthStrikeAvailable().ToInt(); //0 if false, 1 if true
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<ConsecratedWaterProjectile>(), damage, knockBack, player.whoAmI, ai1: strikeValue);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 100);
			recipe.AddRecipeGroup("AnyAdamantiteBar", 5);
			recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

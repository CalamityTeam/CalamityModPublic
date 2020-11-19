using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class HellsSun : RogueWeapon
    {
        private static int damage = 63;
        private static int knockBack = 5;
        private static float SdamageMult = 0.6f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell's Sun");
            Tooltip.SetDefault("The Subterranean Sun in the palm of your hand.\n" +
				"Shoots a gravity-defying spiky ball. Stacks up to 10.\n" +
                "Once stationary, periodically emits small suns that explode on hit\n" +
                "Stealth strikes emit suns at a faster rate and last for a longer amount of time\n" +
				"Right click to delete all existing spiky balls");
        }

        public override void SafeSetDefaults()
        {
            item.damage = damage;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = knockBack;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.maxStack = 10;

            item.shootSpeed = 5f;
            item.shoot = ModContent.ProjectileType<HellsSunProj>();
        }

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.shoot = 0;
				item.shootSpeed = 0f;
				return player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()] > 0;
			}
			else
			{
				item.shoot = ModContent.ProjectileType<HellsSunProj>();
				item.shootSpeed = 5f;
				int UseMax = item.stack;
				return player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()] < UseMax;
			}
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
			modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<HellsSunProj>(), (int)(damage * SdamageMult), knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                Main.projectile[stealth].penetrate = -1;
                Main.projectile[stealth].timeLeft = 2400;
                return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
			modPlayer.killSpikyBalls = true;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SpikyBall, 100);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

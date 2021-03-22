using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class GodsParanoia : RogueWeapon
    {
        private static int damage = 60;
        private static int knockBack = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God's Paranoia");
            Tooltip.SetDefault(@"Shoots a speedy homing spiky ball. Stacks up to 10.
Attaches to enemies and summons a localized storm of god slayer kunai
Stealth strikes home in faster and summon kunai at a faster rate
Right click to delete all existing spiky balls");
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
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.maxStack = 10;

            item.shootSpeed = 5f;
            item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();

        }

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.shoot = ProjectileID.None;
				item.shootSpeed = 0f;
				return player.ownedProjectileCounts[ModContent.ProjectileType<GodsParanoiaProj>()] > 0;
			}
			else
			{
				item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();
				item.shootSpeed = 5f;
				int UseMax = item.stack;
				return player.ownedProjectileCounts[ModContent.ProjectileType<GodsParanoiaProj>()] < UseMax;
			}
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
			modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
				if (stealth.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth].Calamity().stealthStrike = true;
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

            recipe.AddIngredient(ItemID.SpikyBall, 200);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

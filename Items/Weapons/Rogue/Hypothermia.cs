using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class Hypothermia : RogueWeapon
    {
		private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hypothermia");
            Tooltip.SetDefault("Fires a constant barrage of black ice shards\n" +
                               "Stealth strikes additionally fire a short ranged ice chunk that shatters into ice shards");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.height = 32;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item9;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;

            item.damage = 369;
            item.useAnimation = 21;
            item.useTime = 3;
            item.reuseDelay = 1;
            item.crit = 16;
            item.knockBack = 3f;
            item.shoot = ModContent.ProjectileType<HypothermiaShard>();
            item.shootSpeed = 8f;

            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable() && counter == 0) //setting up the stealth strikes
			{
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<HypothermiaChunk>(), damage, knockBack, player.whoAmI);
                Main.projectile[stealth].Calamity().stealthStrike = true;
            }
			int projAmt = Main.rand.Next(1, 3);
			for (int index = 0; index < projAmt; ++index)
			{
				float SpeedX = speedX + Main.rand.NextFloat(-2f, 2f);
				float SpeedY = speedY + Main.rand.NextFloat(-2f, 2f);
				Projectile.NewProjectile(position, new Vector2(SpeedX, SpeedY), type, damage, knockBack, player.whoAmI, Main.rand.Next(4), 0f);
			}

			counter++;
			if (counter >= item.useAnimation / item.useTime)
				counter = 0;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 24);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 6);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

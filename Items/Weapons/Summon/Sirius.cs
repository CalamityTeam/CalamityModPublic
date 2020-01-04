using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Sirius : ModItem
    {
		int siriusSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sirius");
            Tooltip.SetDefault("Summons the brightest star in the night sky to shine upon your foes\n" +
                               "Consumes all of the remaining minion slots on use\n" +
                               "Increased power based on the number of minion slots used");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.height = 62;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item44;

            item.summon = true;
            item.mana = 60;
            item.damage = 500;
            item.knockBack = 3f;
            item.autoReuse = true;
            item.useTime = 36;
            item.useAnimation = 36;
            item.shoot = ModContent.ProjectileType<SiriusMinion>();
            item.shootSpeed = 10f;

            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool CanUseItem(Player player)
		{
			return siriusSlots >= 1;
		}

		public override void HoldItem(Player player)
        {
			double minionCount = 0;
			for (int j = 0; j < Main.projectile.Length; j++)
			{
				if (Main.projectile[j].active && Main.projectile[j].owner == player.whoAmI && Main.projectile[j].minion)
				{
					minionCount += Main.projectile[j].minionSlots;
				}
			}
			siriusSlots = (int)((double)player.maxMinions - minionCount);
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active && projectile.owner == player.whoAmI && projectile.type == ModContent.ProjectileType<SiriusMinion>())
                {
                    projectile.Kill();
                }
            }
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, siriusSlots);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SunGodStaff>());
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.MinionNPCTargetAim();
            }
            return base.UseItem(player);
        }
    }
}

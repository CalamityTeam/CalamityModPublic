using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityMod.Items.Weapons.Melee
{
	public class CosmicShiv : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Shiv");
			Tooltip.SetDefault("Fires a cosmic beam that home in on enemies\n" +
                               "Upon hitting an enemy, a barrage of offscreen objects home in on the enemy as well as raining stars\n" +
                               "'Definintely don't underestimate the power of shivs'");
		}

		public override void SetDefaults()
        {
            item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 23;
			item.useTime = 23;
			item.width = 44;
			item.height = 44;

			item.damage = 720;

			item.melee = true;
			item.knockBack = 9f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("CosmicShivBall");
			item.shootSpeed = 14f;
			item.value = Item.buyPrice(2, 50, 0, 0); //50 gold
            item.Calamity().postMoonLordRarity = 21;
        }
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ModContent.ItemType<ElementalShortsword>());
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());

            recipe.SetResult(this);
	        recipe.AddRecipe();
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            for (int k = 0; k < 36; k++)
            {
                int dustID = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 173, 0f, 0f, 0, default, 1f);
                Main.dust[dustID].velocity *= 3f;
                Main.dust[dustID].scale *= 2f;
            }

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 420);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 420);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 420);
            target.AddBuff(ModContent.BuffType<Plague>(), 420);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 360);
        }
	}
}

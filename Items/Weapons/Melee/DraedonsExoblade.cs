using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class DraedonsExoblade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exoblade");
			Tooltip.SetDefault("Ancient blade of Yharim's weapons and armors expert, Draedon\n" +
							   "Fires an exo beam that homes in on the player and explodes\n" +
							   "Striking an enemy with the blade causes several comets to fire\n" +
							   "All attacks briefly freeze enemies hit\n" +
							   "Enemies hit at very low HP explode into frost energy and freeze nearby enemies\n" +
							   "The lower your HP the more damage this blade does and heals the player on enemy hits");
		}

        public override void SetDefaults()
        {
            item.width = 80;
            item.damage = 5000;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 14;
            item.useTurn = true;
            item.melee = true;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 114;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<Exobeam>();
            item.shootSpeed = 19f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        // Gains 100% of missing health as base damage.
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			int lifeAmount = player.statLifeMax2 - player.statLife;
			flat += lifeAmount * player.MeleeDamage();
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(4))
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107, 0f, 0f, 100, new Color(0, 255, 255));
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= (target.lifeMax * 0.05f))
			{
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<Exoboom>(), (int)(item.damage * player.MeleeDamage()), knockback, Main.myPlayer);
			}
			target.ExoDebuffs();
			Main.PlaySound(SoundID.Item88, player.Center);
			float xPos = player.position.X + 800 * Main.rand.NextBool(2).ToDirectionInt();
			float yPos = player.position.Y + Main.rand.Next(-800, 801);
			Vector2 startPos = new Vector2(xPos, yPos);
			Vector2 velocity = target.position - startPos;
			float dir = 10 / startPos.X;
			velocity.X *= dir * 150;
			velocity.Y *= dir * 150;
			velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
			velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Exocomet>()] < 8)
			{
				for (int comet = 0; comet < 2; comet++)
				{
					float ai1 = Main.rand.NextFloat() + 0.5f;
					Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<Exocomet>(), (int)(item.damage * player.MeleeDamage()), knockback, player.whoAmI, 0f, ai1);
				}
			}
			if (target.type == NPCID.TargetDummy || !target.canGhostHeal || player.moonLeech)
			{
				return;
			}
			int healAmount = Main.rand.Next(5) + 5;
			player.statLife += healAmount;
			player.HealEffect(healAmount);
		}

		public override void OnHitPvp(Player player, Player target, int damage, bool crit)
		{
			if (target.statLife <= (target.statLifeMax2 * 0.05f))
			{
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<Exoboom>(), (int)(item.damage * player.MeleeDamage()), item.knockBack, Main.myPlayer);
			}
			target.ExoDebuffs();
			Main.PlaySound(SoundID.Item88, player.Center);
			float xPos = player.position.X + 800 * Main.rand.NextBool(2).ToDirectionInt();
			float yPos = player.position.Y + Main.rand.Next(-800, 801);
			Vector2 startPos = new Vector2(xPos, yPos);
			Vector2 velocity = target.position - startPos;
			float dir = 10 / startPos.X;
			velocity.X *= dir * 150;
			velocity.Y *= dir * 150;
			velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
			velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Exocomet>()] < 8)
			{
				for (int comet = 0; comet < 2; comet++)
				{
					float ai1 = Main.rand.NextFloat() + 0.5f;
					Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<Exocomet>(), (int)(item.damage * player.MeleeDamage()), item.knockBack, player.whoAmI, 0f, ai1);
				}
			}
			if (player.moonLeech)
				return;
			int healAmount = Main.rand.Next(5) + 5;
			player.statLife += healAmount;
			player.HealEffect(healAmount);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Terratomere>());
			recipe.AddIngredient(ModContent.ItemType<AnarchyBlade>());
			recipe.AddIngredient(ModContent.ItemType<FlarefrostBlade>());
			recipe.AddIngredient(ModContent.ItemType<PhoenixBlade>());
			recipe.AddIngredient(ModContent.ItemType<StellarStriker>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}

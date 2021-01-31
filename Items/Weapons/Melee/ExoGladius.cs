using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ExoGladius : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Gladius");
            Tooltip.SetDefault("Do not underestimate the power of Exoblade's younger brother\n" +
                "Striking an enemy with the blade makes you immune for a short time and summons comets from the sky\n" +
                "Fires a rainbow orb that summons sword beams on hit");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 12;
            item.useTime = 12;
            item.width = 56;
            item.height = 56;
            item.damage = 2700;
            item.melee = true;
            item.knockBack = 9.9f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<ExoGladProj>();
            item.shootSpeed = 19f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, item.shootSpeed * player.direction, 0f, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalileoGladius>());
            recipe.AddIngredient(ModContent.ItemType<CosmicShiv>());
            recipe.AddIngredient(ModContent.ItemType<Lucrecia>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
            }
        }

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.ExoDebuffs();
			OnHitEffects(player);
		}

		public override void OnHitPvp(Player player, Player target, int damage, bool crit)
		{
			target.ExoDebuffs();
			OnHitEffects(player);
		}

        private void OnHitEffects(Player player)
        {
			bool isImmune = false;
			for (int j = 0; j < player.hurtCooldowns.Length; j++)
			{
				if (player.hurtCooldowns[j] > 0)
					isImmune = true;
			}
			if (!isImmune)
            {
                player.immune = true;
                if (player.immuneTime < item.useTime)
                    player.immuneTime = item.useTime;
                if (player.hurtCooldowns[0] < item.useTime)
                    player.hurtCooldowns[0] = item.useTime;
                if (player.hurtCooldowns[1] < item.useTime)
                    player.hurtCooldowns[1] = item.useTime;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                int damage = player.GetWeaponDamage(player.ActiveItem());
                CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 25f, ModContent.ProjectileType<ExoGladComet>(), damage, 15f, player.whoAmI);
            }
        }
	}
}

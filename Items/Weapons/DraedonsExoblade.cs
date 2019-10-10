using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class DraedonsExoblade : ModItem
    {
		private static int BaseDamage = 6700;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exoblade");
            Tooltip.SetDefault("Ancient blade of Yharim's weapons and armors expert, Draedon\n" +
                               "Fires an exo beam that homes in on the player and explodes\n" +
                               "Striking an enemy with the blade causes several comets to fire\n" +
                               "All attacks freeze any enemy in place for several seconds at a 10% chance\n" +
                               "Enemies hit at very low HP explode into frost energy and freeze nearby enemies\n" +
                               "The lower your HP the more damage this blade does and heals the player on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.damage = BaseDamage;
            item.useAnimation = 14;
            item.useStyle = 1;
            item.useTime = 14;
            item.useTurn = true;
            item.melee = true;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 114;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Exobeam");
            item.shootSpeed = 19f;
			item.Calamity().postMoonLordRarity = 15;
		}

		// Gains 100% of missing health as base damage.
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			int lifeAmount = player.statLifeMax2 - player.statLife;
			flat += lifeAmount * player.meleeDamage;
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
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("Exoboom"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
            }
			target.AddBuff(mod.BuffType("ExoFreeze"), 30);
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
			target.AddBuff(mod.BuffType("GlacialState"), 120);
			target.AddBuff(mod.BuffType("Plague"), 120);
			target.AddBuff(mod.BuffType("HolyLight"), 120);
			target.AddBuff(BuffID.CursedInferno, 120);
			target.AddBuff(BuffID.Frostburn, 120);
			target.AddBuff(BuffID.OnFire, 120);
			target.AddBuff(BuffID.Ichor, 120);
			Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 88);
            float xPos = (Main.rand.NextBool(2)) ? player.position.X + 800 : player.position.X - 800;
            Vector2 vector2 = new Vector2(xPos, player.position.Y + Main.rand.Next(-800, 801));
            float num80 = xPos;
            float speedX = (float)target.position.X - vector2.X;
            float speedY = (float)target.position.Y - vector2.Y;
            float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
            dir = 10 / num80;
            speedX *= dir * 150;
            speedY *= dir * 150;
            if (speedX > 15f)
            {
                speedX = 15f;
            }
            if (speedX < -15f)
            {
                speedX = -15f;
            }
            if (speedY > 15f)
            {
                speedY = 15f;
            }
            if (speedY < -15f)
            {
                speedY = -15f;
            }
            if (player.ownedProjectileCounts[mod.ProjectileType("Exocomet")] < 8)
            {
                for (int comet = 0; comet < 2; comet++)
                {
                    float ai1 = (Main.rand.NextFloat() + 0.5f);
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("Exocomet"), (int)((float)item.damage * player.meleeDamage), knockback, player.whoAmI, 0f, ai1);
                }
            }
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
            {
                return;
            }
            int healAmount = (Main.rand.Next(5) + 5);
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Terratomere");
            recipe.AddIngredient(null, "AnarchyBlade");
            recipe.AddIngredient(null, "FlarefrostBlade");
            recipe.AddIngredient(null, "PhoenixBlade");
            recipe.AddIngredient(null, "StellarStriker");
            recipe.AddIngredient(null, "NightmareFuel", 5);
            recipe.AddIngredient(null, "EndothermicEnergy", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

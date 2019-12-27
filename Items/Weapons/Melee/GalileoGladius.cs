using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GalileoGladius : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galileo Gladius");
            Tooltip.SetDefault("Don't underestimate the power of small space swords\n" +
                "Shoots a homing crescent moon\n" +
				"Spawns planetoids on enemy hits");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 10;
            item.useTime = 10;
            item.width = 44;
            item.height = 44;
            item.damage = 225;
            item.melee = true;
            item.knockBack = 10f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GalileosMoon>();
            item.shootSpeed = 14f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, item.shootSpeed * player.direction, 0f, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, (Main.rand.NextBool(2) ? 20 : 176), (float)(player.direction * 2), 0f, 150, default, 1.3f);
                Main.dust[num250].velocity *= 0.2f;
                Main.dust[num250].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            float x = player.position.X + (float)Main.rand.Next(-400, 400);
            float y = player.position.Y - (float)Main.rand.Next(500, 800);
            Vector2 vector = new Vector2(x, y);
            float num15 = player.position.X + (float)(player.width / 2) - vector.X;
            float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
            num15 += (float)Main.rand.Next(-100, 101);
            int num17 = 25;
            float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
            num18 = (float)num17 / num18;
            num15 *= num18;
            num16 *= num18;
            if (player.whoAmI == Main.myPlayer)
            {
				if (player.Calamity().galileoCooldown <= 0)
				{
					int num19 = Projectile.NewProjectile(x, y, num15, num16, ModContent.ProjectileType<GalileosPlanet>(), (int)((float)item.damage * player.meleeDamage * 2f), 15f, player.whoAmI, 0f, 0f);
					Main.projectile[num19].ai[1] = player.position.Y;
					player.Calamity().galileoCooldown = 15;
				}
            }
        }
    }
}

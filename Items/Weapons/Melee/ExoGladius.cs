using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
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
            item.useStyle = 3;
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
            item.rare = 10;
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
            if (!player.immune)
            {
                player.immune = true;
				if (player.immuneTime < 10)
					player.immuneTime = 10;
				if (player.hurtCooldowns[0] < 10)
					player.hurtCooldowns[0] = 10;
				if (player.hurtCooldowns[1] < 10)
					player.hurtCooldowns[1] = 10;
            }

            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);

            float x = target.Center.X + (float)Main.rand.Next(-400, 400);
            float y = target.Center.Y - (float)Main.rand.Next(500, 800);
            Vector2 vector = new Vector2(x, y);
            float num15 = target.Center.X + (float)(target.width / 2) - vector.X;
            float num16 = target.Center.Y + (float)(target.height / 2) - vector.Y;
            num15 += (float)Main.rand.Next(-100, 101);
            int num17 = 25;
            float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
            num18 = (float)num17 / num18;
            num15 *= num18;
            num16 *= num18;
            if (player.whoAmI == Main.myPlayer)
            {
				Projectile.NewProjectile(x, y, num15, num16, ModContent.ProjectileType<ExoGladComet>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), 15f, player.whoAmI, 0f, 0f);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (!player.immune)
            {
                player.immune = true;
				if (player.immuneTime < 10)
					player.immuneTime = 10;
				if (player.hurtCooldowns[0] < 10)
					player.hurtCooldowns[0] = 10;
				if (player.hurtCooldowns[1] < 10)
					player.hurtCooldowns[1] = 10;
            }

            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);

            float x = target.Center.X + (float)Main.rand.Next(-400, 400);
            float y = target.Center.Y - (float)Main.rand.Next(500, 800);
            Vector2 vector = new Vector2(x, y);
            float num15 = target.Center.X + (float)(target.width / 2) - vector.X;
            float num16 = target.Center.Y + (float)(target.height / 2) - vector.Y;
            num15 += (float)Main.rand.Next(-100, 101);
            int num17 = 25;
            float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
            num18 = (float)num17 / num18;
            num15 *= num18;
            num16 *= num18;
            if (player.whoAmI == Main.myPlayer)
            {
				Projectile.NewProjectile(x, y, num15, num16, ModContent.ProjectileType<ExoGladComet>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), 15f, player.whoAmI, 0f, 0f);
            }
        }
    }
}

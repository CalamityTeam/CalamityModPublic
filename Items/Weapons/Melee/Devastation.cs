using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Devastation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devastation");
            Tooltip.SetDefault("Fires galaxy blasts that explode");
        }

        public override void SetDefaults()
        {
            item.width = 72;
            item.damage = 160;
            item.melee = true;
            item.useAnimation = 24;
            item.useTime = 24;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 72;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<GalaxyBlast>();
            item.shootSpeed = 16f;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            switch (Main.rand.Next(6))
            {
                case 1:
                    type = ModContent.ProjectileType<GalaxyBlast>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<GalaxyBlastType2>();
                    break;
                case 3:
                    type = ModContent.ProjectileType<GalaxyBlastType3>();
                    break;
                default:
                    break;
            }
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);

			Projectile blast = CalamityUtils.ProjectileToMouse(player, 4, item.shootSpeed, 0f, 40f, ModContent.ProjectileType<GalaxyBlast>(), damage / 2, knockBack, player.whoAmI, true);
			blast.ai[1] = Main.rand.Next(5);
			Projectile blast2 = CalamityUtils.ProjectileToMouse(player, 4, item.shootSpeed, 0f, 40f, ModContent.ProjectileType<GalaxyBlastType2>(), damage / 2, knockBack, player.whoAmI, true);
			blast2.ai[1] = Main.rand.Next(3);
			CalamityUtils.ProjectileToMouse(player, 4, item.shootSpeed, 0f, 40f, ModContent.ProjectileType<GalaxyBlastType3>(), damage / 2, knockBack, player.whoAmI, true);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CatastropheClaymore>());
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 10);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 73);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}

using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
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
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 10;
            item.useTime = 10;
            item.width = 44;
            item.height = 44;
            item.damage = 110;
            item.melee = true;
            item.knockBack = 10f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GalileosMoon>();
            item.shootSpeed = 14f;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int moonDamage = (int)(damage * 0.3333f);
            Projectile.NewProjectile(position.X, position.Y, item.shootSpeed * player.direction, 0f, type, moonDamage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 8);
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
            SpawnMeteor(player);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            SpawnMeteor(player);
        }

        private void SpawnMeteor(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.Calamity().galileoCooldown <= 0)
                {
                    int damage = player.GetWeaponDamage(player.ActiveItem()) * 2;
                    CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 25f, ModContent.ProjectileType<GalileosPlanet>(), damage, 15f, player.whoAmI);
                    player.Calamity().galileoCooldown = 15;
                }
            }
        }
    }
}

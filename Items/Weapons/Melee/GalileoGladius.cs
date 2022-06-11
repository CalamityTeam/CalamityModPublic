using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.width = 44;
            Item.height = 44;
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GalileosMoon>();
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int moonDamage = (int)(damage * 0.3333f);
            Projectile.NewProjectile(source, position.X, position.Y, Item.shootSpeed * player.direction, 0f, type, moonDamage, knockback, player.whoAmI, 0f, 0f);
            return false;
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
                var source = player.GetSource_ItemUse(Item);
                if (player.Calamity().galileoCooldown <= 0)
                {
                    int damage = player.GetWeaponDamage(player.ActiveItem()) * 2;
                    CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 25f, ModContent.ProjectileType<GalileosPlanet>(), damage, 15f, player.whoAmI);
                    player.Calamity().galileoCooldown = 15;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Gladius). //This may be too rare for this recipe, we'll see I guess. - Merkalto
                AddIngredient<Lumenyl>(8).
                AddIngredient<RuinousSoul>(5).
                AddIngredient<ExodiumClusterOre>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CatastropheClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe Claymore");
            Tooltip.SetDefault("Fires explosive energy bolts");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.damage = 98;
            item.melee = true;
            item.useAnimation = 23;
            item.useTime = 23;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 56;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<CalamityAura>();
            item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.ProjectileType<CalamityAura>(),
                ModContent.ProjectileType<CalamityAuraType2>(),
                ModContent.ProjectileType<CalamityAuraType3>()
            });

            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddIngredient(ItemID.CrystalShard, 7);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddRecipeGroup("CursedFlameIchor", 5);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
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
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Ichor, 60);
                target.AddBuff(BuffID.OnFire, 180);
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Ichor, 60);
                target.AddBuff(BuffID.OnFire, 180);
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }
    }
}

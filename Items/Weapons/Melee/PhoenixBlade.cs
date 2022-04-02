using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PhoenixBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phoenix Blade");
            Tooltip.SetDefault("Enemies explode and emit healing flames on death");
        }

        public override void SetDefaults()
        {
            item.width = 106;
            item.damage = 160;
            item.melee = true;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 27;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 106;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = ItemRarityID.LightPurple;
            item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
                OnHitEffects(player, target.Center, knockback, damage, crit);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
                OnHitEffects(player, target.Center, item.knockBack, damage, crit);
        }

        private void OnHitEffects(Player player, Vector2 targetPos, float kBack, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            int boom = Projectile.NewProjectile(targetPos, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), damage, kBack, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            if (boom.WithinBounds(Main.maxProjectiles))
                Main.projectile[boom].Calamity().forceMelee = true;

            float randomSpeedX = Main.rand.Next(5);
            float randomSpeedY = Main.rand.Next(3, 7);
            Projectile.NewProjectile(targetPos.X, targetPos.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), damage, kBack, player.whoAmI);
            Projectile.NewProjectile(targetPos.X, targetPos.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), damage, kBack, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 244);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BreakerBlade);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

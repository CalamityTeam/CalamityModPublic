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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 106;
            Item.damage = 160;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 27;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 106;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
                OnHitEffects(player, target.Center, knockback, damage, crit);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
                OnHitEffects(player, target.Center, Item.knockBack, damage, crit);
        }

        private void OnHitEffects(Player player, Vector2 targetPos, float kBack, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            int boom = Projectile.NewProjectile(source, targetPos, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), damage, kBack, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            if (boom.WithinBounds(Main.maxProjectiles))
                Main.projectile[boom].Calamity().forceMelee = true;

            float randomSpeedX = Main.rand.Next(5);
            float randomSpeedY = Main.rand.Next(3, 7);
            Projectile.NewProjectile(source, targetPos.X, targetPos.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), damage, kBack, player.whoAmI);
            Projectile.NewProjectile(source, targetPos.X, targetPos.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), damage, kBack, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 244);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BreakerBlade).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient<EssenceofCinder>().
                AddIngredient(ItemID.SoulofMight, 3).
                AddIngredient(ItemID.SoulofSight, 3).
                AddIngredient(ItemID.SoulofFright, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

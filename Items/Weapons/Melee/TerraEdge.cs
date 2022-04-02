using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TerraEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Edge");
            Tooltip.SetDefault("Heals the player on enemy hits\n" +
                "Fires a beam that inflicts ichor for a short time");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 58;
            item.scale = 1.5f;
            item.damage = 112;
            item.melee = true;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 17;
            item.useTurn = true;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<TerraEdgeBeam>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 0.75), knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TrueBloodyEdge>());
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TrueNightsEdge);
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 90);

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(2) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 90);

            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(2) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }
    }
}

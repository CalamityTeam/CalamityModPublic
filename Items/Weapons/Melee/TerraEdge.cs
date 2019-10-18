using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

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
            item.damage = 112;
            item.melee = true;
            item.useAnimation = 17;
            item.useStyle = 1;
            item.useTime = 17;
            item.useTurn = true;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<TerraEdgeBeam>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueBloodyEdge");
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(null, "LivingShard", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TrueNightsEdge);
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(null, "LivingShard", 7);
            recipe.AddTile(TileID.MythrilAnvil);
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
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
            {
                return;
            }
            int healAmount = Main.rand.Next(2) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }
    }
}

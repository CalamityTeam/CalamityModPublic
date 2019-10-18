using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Tools
{
    public class FlamebeakHampick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flamebeak Hampick");
            Tooltip.SetDefault("Capable of mining Lihzahrd Bricks");
        }

        public override void SetDefaults()
        {
            item.damage = 58;
            item.melee = true;
            item.width = 52;
            item.height = 50;
            item.useTime = 6;
            item.useAnimation = 15;
            item.useTurn = true;
            item.pick = 210;
            item.hammer = 130;
            item.useStyle = 1;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 127);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}

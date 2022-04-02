using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class ChaoswarpedSlashaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tectonic Truncator");
        }

        public override void SetDefaults()
        {
            item.damage = 68;
            item.knockBack = 7f;
            item.useTime = 6;
            item.useAnimation = 17;
            item.axe = 190 / 5;
            item.tileBoost += 2;

            item.melee = true;
            item.width = 50;
            item.height = 50;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? 16 : 127);
            }
            if (Main.rand.NextBool(5))
            {
                int smoke = Gore.NewGore(new Vector2(hitbox.X, hitbox.Y), default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ChaoswarpedSlashaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaoswarped Slashaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 68;
            item.melee = true;
            item.width = 50;
            item.height = 50;
            item.useTime = 31;
            item.useAnimation = 31;
            item.useTurn = true;
            item.axe = 40;
            item.useStyle = 1;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 9);
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

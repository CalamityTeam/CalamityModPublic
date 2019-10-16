using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BloodyEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Edge");
            Tooltip.SetDefault("Chance to heal the player on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.damage = 52;
            item.melee = true;
            item.useAnimation = 23;
            item.useStyle = 1;
            item.useTime = 23;
            item.knockBack = 5.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
            item.height = 60;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LightsBane);
            recipe.AddIngredient(ItemID.BladeofGrass);
            recipe.AddIngredient(ItemID.Muramasa);
            recipe.AddIngredient(ItemID.FieryGreatsword);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BloodButcherer);
            recipe.AddIngredient(ItemID.BladeofGrass);
            recipe.AddIngredient(ItemID.Muramasa);
            recipe.AddIngredient(ItemID.FieryGreatsword);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 5);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
            {
                return;
            }
            int healAmount = Main.rand.Next(3) + 1;
            if (Main.rand.NextBool(2))
            {
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            }
        }
    }
}

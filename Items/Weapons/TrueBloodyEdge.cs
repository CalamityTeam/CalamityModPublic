using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TrueBloodyEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Bloody Edge");
            Tooltip.SetDefault("Chance to heal the player on enemy hits\n" +
				"Fires a bloody blade");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.damage = 70;
            item.melee = true;
            item.useAnimation = 24;
            item.useStyle = 1;
            item.useTime = 24;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = mod.ProjectileType("BloodyBlade");
            item.shootSpeed = 11f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodyEdge");
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 5);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            int healAmount = (Main.rand.Next(6) + 1);
            if (Main.rand.Next(2) == 0)
            {
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            }
        }
    }
}

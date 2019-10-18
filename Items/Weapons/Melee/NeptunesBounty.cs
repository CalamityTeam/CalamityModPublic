using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Weapons.Melee
{
    public class NeptunesBounty : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Neptune's Bounty");
            Tooltip.SetDefault("Hitting enemies will cause the crush depth debuff\n" +
                "The lower the enemies' defense the more damage they take from this debuff\n" +
                "Fires a trident that rains additional tridents as it travels");
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.damage = 380;
            item.melee = true;
            item.useAnimation = 22;
            item.useTime = 22;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 80;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<NeptuneOrb>();
            item.shootSpeed = 12f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AbyssBlade");
            recipe.AddIngredient(null, "RuinousSoul", 5);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
        }
    }
}

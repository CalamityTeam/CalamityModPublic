using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AstralPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Pickaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 65;
            item.crit += 25;
            item.melee = true;
            item.width = 50;
            item.height = 60;
            item.useTime = 5;
            item.useAnimation = 10;
            item.useTurn = true;
            item.pick = 220;
            item.useStyle = 1;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AstralBar", 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Dust d = CalamityGlobalItem.MeleeDustHelper(player, Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>(), 0.56f, 40, 65, -0.13f, 0.13f);
            if (d != null)
            {
                d.customData = 0.02f;
            }
        }
    }
}

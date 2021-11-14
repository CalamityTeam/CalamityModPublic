using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Tools
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
            item.knockBack = 5f;
            item.useTime = 6;
            item.useAnimation = 10;
            item.pick = 220;
            item.tileBoost += 3;

            item.crit += 25;
            item.melee = true;
            item.width = 50;
            item.height = 60;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 7);
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
            Dust d = CalamityUtils.MeleeDustHelper(player, Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>(), 0.56f, 40, 65, -0.13f, 0.13f);
            if (d != null)
            {
                d.customData = 0.02f;
            }
        }
    }
}

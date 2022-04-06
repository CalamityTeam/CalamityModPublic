using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class Gelpick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gelpick");
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.knockBack = 2.5f;
            Item.useTime = 10;
            Item.useAnimation = 20;
            Item.pick = 100;
            Item.tileBoost += 1;

            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 46;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(15).
                AddIngredient(ItemID.Gel, 30).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddTile<StaticRefiner>().
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 20);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }
    }
}

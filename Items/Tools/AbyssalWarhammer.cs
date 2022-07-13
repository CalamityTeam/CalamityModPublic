using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class AbyssalWarhammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abyssal Warhammer");
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.knockBack = 8f;
            Item.useTime = 10;
            Item.useAnimation = 38;
            Item.hammer = 88;
            Item.tileBoost += 1;

            Item.DamageType = DamageClass.Melee;
            Item.width = 70;
            Item.height = 70;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 29);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}

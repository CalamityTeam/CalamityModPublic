using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CausticEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caustic Edge");
            Tooltip.SetDefault("Inflicts poison and venom on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.damage = 70;
            Item.scale = 1.4f;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 27;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 48;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 240);
            target.AddBuff(BuffID.Venom, 120);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 240);
            target.AddBuff(BuffID.Venom, 120);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BladeofGrass).
                AddIngredient(ItemID.LavaBucket).
                AddRecipeGroup("Boss2Material", 3).
                AddIngredient(ItemID.Deathweed, 5).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}

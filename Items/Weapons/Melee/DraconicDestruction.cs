using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DraconicDestruction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Destruction");
            Tooltip.SetDefault("Fires a draconic sword beam that explodes into additional beams\n" +
                "Additional beams fly up and down to shred enemies");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.damage = 80;
            item.melee = true;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 24;
            item.useTurn = true;
            item.knockBack = 7.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 94;
            item.shoot = ModContent.ProjectileType<DracoBeam>();
            item.shootSpeed = 14f;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 3);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 35);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 600);
        }
    }
}

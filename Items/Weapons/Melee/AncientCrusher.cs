using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AncientCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Crusher");
            Tooltip.SetDefault("Summons fossil spikes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 55;
            item.melee = true;
            item.useAnimation = 30;
            item.useStyle = 1;
            item.useTime = 30;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Amber, 8);
            recipe.AddIngredient(ItemID.FossilOre, 35);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FossilSpike>(), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
        }
    }
}

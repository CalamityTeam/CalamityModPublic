using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BalefulHarvester : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baleful Harvester");
            Tooltip.SetDefault("Shoots skulls that split into homing fire orbs on enemy hits\n" +
                "Summons flaming pumpkins on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 160;
            item.width = 66;
            item.height = 66;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.shoot = ModContent.ProjectileType<BalefulHarvesterProjectile>();
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TheHorsemansBlade);
            recipe.AddIngredient(ItemID.BookofSkulls);
            recipe.AddIngredient(ItemID.FragmentSolar, 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            CalamityGlobalItem.HorsemansBladeOnHit(player, target.whoAmI, (int)(item.damage * player.meleeDamage * 1.5f), knockback, false);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}

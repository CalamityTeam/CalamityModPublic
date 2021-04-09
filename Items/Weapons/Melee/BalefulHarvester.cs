using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
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
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, target.whoAmI, (int)(item.damage * player.MeleeDamage() * 1.5f), knockback);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            CalamityPlayerOnHit.HorsemansBladeOnHit(player, -1, (int)(item.damage * player.MeleeDamage() * 1.5f), item.knockBack);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}

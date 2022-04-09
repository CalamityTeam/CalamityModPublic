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
            Tooltip.SetDefault("Summons flaming pumpkins and skulls that split into homing fire orbs on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.width = 66;
            Item.height = 66;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            int type = Main.rand.NextBool() ? ModContent.ProjectileType<BalefulHarvesterProjectile>() : ProjectileID.FlamingJack;
            CalamityPlayer.HorsemansBladeOnHit(player, target.whoAmI, (int)(damage * 1.5f), knockback, 0, type);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            int type = Main.rand.NextBool() ? ModContent.ProjectileType<BalefulHarvesterProjectile>() : ProjectileID.FlamingJack;
            CalamityPlayer.HorsemansBladeOnHit(player, -1, (int)(damage * 1.5f), Item.knockBack, 0, type);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TheHorsemansBlade).
                AddIngredient(ItemID.BookofSkulls).
                AddIngredient(ItemID.FragmentSolar, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

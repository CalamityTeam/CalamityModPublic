using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Leviathan
{
    public class LureofEnthrallment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearl of Enthrallment");
            Tooltip.SetDefault("Summons a siren to fight for you\n" +
                "The siren stays above you, shooting water spears, ice mist, and treble clefs at nearby enemies");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sirenWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("SirenLure")) == -1)
                {
                    player.AddBuff(mod.BuffType("SirenLure"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("SirenLure")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("SirenLure"), (int)(65f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "IOU");
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

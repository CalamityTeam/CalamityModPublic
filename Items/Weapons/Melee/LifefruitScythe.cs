using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class LifefruitScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lifehunt Scythe");
            Tooltip.SetDefault("Heals you on hit and shoots an energy scythe");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 156;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.height = 72;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.shoot = ModContent.ProjectileType<LifeScythe>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 75);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (!target.canGhostHeal || player.moonLeech)
                return;

            player.statLife += 5;
            player.HealEffect(5);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (player.moonLeech)
                return;

            player.statLife += 5;
            player.HealEffect(5);
        }
    }
}

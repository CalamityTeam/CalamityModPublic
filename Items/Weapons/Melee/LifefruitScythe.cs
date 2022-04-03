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
            Item.width = 62;
            Item.damage = 156;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.height = 72;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shoot = ModContent.ProjectileType<LifeScythe>();
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<UeliaceBar>(), 15).AddTile(TileID.LunarCraftingStation).Register();
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

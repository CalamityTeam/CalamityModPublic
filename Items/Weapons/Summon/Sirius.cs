using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Sirius : ModItem
    {
        int siriusSlots;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sirius");
            Tooltip.SetDefault("Summons the brightest star in the night sky to shine upon your foes\n" +
                               "Consumes all of the remaining minion slots on use\n" +
                               "Must be used from the hotbar\n" +
                               "Increased power based on the number of minion slots used");
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item44;

            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.damage = 160;
            Item.knockBack = 3f;
            Item.useTime = Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<SiriusMinion>();
            Item.shootSpeed = 10f;

            Item.value = Item.buyPrice(1, 40, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void HoldItem(Player player)
        {
            double minionCount = 0;
            for (int j = 0; j < Main.projectile.Length; j++)
            {
                Projectile proj = Main.projectile[j];
                if (proj.active && proj.owner == player.whoAmI && proj.minion && proj.type != Item.shoot)
                {
                    minionCount += proj.minionSlots;
                }
            }
            siriusSlots = (int)(player.maxMinions - minionCount);
        }

        public override bool CanUseItem(Player player)
        {
            return siriusSlots >= 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, siriusSlots, 30f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SunGodStaff>()).AddIngredient(ModContent.ItemType<Lumenite>(), 5).AddIngredient(ModContent.ItemType<RuinousSoul>(), 2).AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}

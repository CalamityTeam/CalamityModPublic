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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 90;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.useTurn = true;
            Item.knockBack = 7.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 90;
            Item.shoot = ModContent.ProjectileType<DracoBeam>();
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>(5).
                AddIngredient<CoreofSunlight>(3).
                AddIngredient<CoreofEleum>(3).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddTile(ModContent.TileType<DraedonsForge>()).
                Register();
        }
    }
}

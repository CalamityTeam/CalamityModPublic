using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AbyssBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyss Blade");
            Tooltip.SetDefault("Fires short-range tridents\n" +
                "Hitting enemies will inflict the crush depth debuff\n" +
                "The lower the enemies' defense, the more damage they take from this debuff");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 26;
            Item.useTime = 26;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<DepthOrb>();
            Item.shootSpeed = 9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 0.75), knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DepthCrusher>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient<DepthCells>(15).
                AddIngredient<Lumenyl>(10).
                AddIngredient<Tenebris>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

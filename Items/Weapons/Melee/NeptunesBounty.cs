using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class NeptunesBounty : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Neptune's Bounty");
            Tooltip.SetDefault("Fires a trident that rains additional tridents as it travels\n" +
                "Hitting enemies will inflict the crush depth debuff\n" +
                "The lower the enemies' defense, the more damage they take from this debuff");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 122;
            Item.height = 122;
            Item.damage = 251;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NeptuneOrb>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
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
                AddIngredient<AbyssBlade>().
                AddIngredient<RuinousSoul>(5).
                AddIngredient<Phantoplasm>(5).
                AddIngredient<DepthCells>(15).
                AddIngredient<Lumenyl>(15).
                AddIngredient<Tenebris>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

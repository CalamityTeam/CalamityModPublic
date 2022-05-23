using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Terratomere : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terratomere");
            Tooltip.SetDefault("Linked to the essence of Terraria\n" +
                               "Heals the player on true melee hits\n" +
                               "Fires a barrage of 4 homing beams that freeze enemies");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.damage = 260;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 21;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 66;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shoot = ModContent.ProjectileType<TerratomereProjectile>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 4; ++index)
            {
                float SpeedX = velocity.X + Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * 0.5), knockback, player.whoAmI, 0f, 0f);
            }

            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueNightsStabber>().
                AddIngredient<TrueExcaliburShortsword>().
                AddIngredient<LivingShard>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

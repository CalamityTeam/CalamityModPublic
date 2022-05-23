using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Brimlash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlash");
            Tooltip.SetDefault("Fires a brimstone bolt that explodes into more bolts on death");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 54;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<BrimlashProj>();
            Item.shootSpeed = 10f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UnholyCore>(4).
                AddIngredient<EssenceofChaos>(3).
                AddIngredient<CalamityDust>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

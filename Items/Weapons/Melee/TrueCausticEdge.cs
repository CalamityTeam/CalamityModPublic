using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    // Thanks a lot for naming it Caustic Edge :)
    public class TrueCausticEdge : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.useTurn = true;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 74;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<TrueCausticEdgeProjectile>();
            Item.shootSpeed = 16f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => damage = (int)(damage * 0.75);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 150);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(BuffID.Venom, 150);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TaintedBlade>().
                AddRecipeGroup("AnyEvilFlask", 5).
                AddIngredient(ItemID.FlaskofPoison, 5).
                AddIngredient(ItemID.Deathweed, 4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}

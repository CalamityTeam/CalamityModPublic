using CalamityMod.Systems.Collections;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FlarefrostBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Frostburn2];
        }
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 66;
            Item.damage = 125;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 29;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<Flarefrost>();
            Item.shootSpeed = 11f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => damage = (int)(damage * 0.6);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustChoice = Main.rand.Next(2);
            if (dustChoice == 0)
            {
                dustChoice = 67;
            }
            else
            {
                dustChoice = 6;
            }
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustChoice);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            target.AddBuff(BuffID.Frostburn2, 180);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            target.AddBuff(BuffID.Frostburn2, 180);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(8).
                AddIngredient(ItemID.HellstoneBar, 8).
                AddIngredient(ItemID.SoulofLight, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

using Terraria.DataStructures;
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
    [LegacyName("ProfanedSword")]
    public class BrimstoneSword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 52;
            Item.scale = 1.5f;
            Item.useAnimation = Item.useTime = 28;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<BrimstoneSwordProj>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.DamageType = DamageClass.MeleeNoSpeed;
                Item.noMelee = true;
            }
            else
            {
                Item.DamageType = DamageClass.Melee;
                Item.noMelee = false;
            }

            return base.UseItem(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
                type = ProjectileID.None;
        }

        public override void UseAnimation(Player player)
        {
            Item.noUseGraphic = false;

            if (player.altFunctionUse == 2)
                Item.noUseGraphic = true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 0.5f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Brimblast>(), Item.damage, Item.knockBack, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Brimblast>(), Item.damage, Item.knockBack, Main.myPlayer);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UnholyCore>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
